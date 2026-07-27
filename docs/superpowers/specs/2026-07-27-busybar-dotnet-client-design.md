# BusyBar .NET HTTP API client — design

Date: 2026-07-27
Status: Approved for planning
Repo: `J:\Projects\Busy\busybar-dotnet` (own git repo, MIT licensed, standalone publishable library)

## Context

This library is the first of two sub-projects. It exists so a separate consumer —
initially a Halo PSA status-dashboard worker service (`J:\Projects\Busy\halo-busybar-agent`,
specced separately) — has a typed, idiomatic .NET client for the
[BUSY Bar HTTP API](https://docs.busy.app/bar/dev/http-api). It is designed to be released
independently (NuGet-ready) and used by other .NET consumers beyond the Halo project.

To keep the .NET library recognizable to anyone who already knows the official clients, its
class/method naming is modelled directly on the official TypeScript library
[`@busy-app/busy-lib`](https://github.com/busy-app/busylib-ts) (npm), which wraps the same
HTTP API. The canonical API reference used for this design is the live OpenAPI document served
at `https://api.busy.app/busybar/openapi.yaml` (captured as `Name=1.1.1-rc`, API version
reported as `25.0.0` inside the document — see Open Questions).

## Scope

**In scope (v1):** a typed wrapper around the BUSY Bar HTTP API only — full parity with the
`BusyBar` class in `busylib-ts`, covering all of its namespaces:

`Account`, `Assets`, `Audio`, `Ble`, `Busy` (Pomodoro/interval timer), `Display`, `Input`,
`Settings`, `SmartHome`, `Storage`, `System`, `Time`, `Update`, `Wifi`.

**Out of scope (v1), explicitly:**
- `StateStream` — real-time protobuf state updates over WebSocket. Different transport/format
  (protobuf, not JSON), no `.proto` schema currently available to this project.
- `ScreenRenderer` — WebGL2 rendering; not applicable outside a browser/UI context.
- Publishing to a real NuGet feed / CI package pipeline — build the package locally
  (`dotnet pack`), don't wire up publishing yet.

Both exclusions can become follow-up specs later if a consumer needs them.

## Architecture

- **.NET 10** (current LTS as of this writing).
- Solution `BusyBar.sln` with two projects:
  - `BusyBar` — the library. Namespace `Busy.Bar`; primary type `BusyBar` (a deliberate,
    accepted quirk: namespace and type share a root name, matching the TS package's single
    exported `BusyBar` class).
  - `BusyBar.Tests` — xUnit test project.
- MIT license file, matching upstream.
- Structured so `dotnet pack` produces a valid NuGet package (`BusyBar` package id) — no feed
  configuration required for v1.

## API surface & naming

Constructor and connection options mirror the TS constructor almost exactly:

```csharp
var bar = new BusyBar(new BusyBarOptions
{
    Addr = "10.0.4.20",            // IP, hostname, or full URL. Same auto-protocol rule as TS:
                                     // http:// by default, https:// when Addr is the cloud host.
    Token = null,                   // Bearer token, for the api.busy.app cloud proxy
    HttpAccessPassword = null,      // sent as x-api-token, for LAN HTTP-access-password mode
    Timeout = TimeSpan.FromSeconds(3) // default per-request timeout, overridable per call
});

bar.SetToken("...");                 // runtime setter, mirrors TS setToken()
bar.SetHttpAccessPassword("...");    // runtime setter, mirrors TS setHTTPAccessPassword()
```

- **Method names**: identical stems to `busylib-ts` (`DisplayDraw`, `DisplayClear`,
  `DisplayBrightnessGet`, `DisplayBrightnessSet`, `SystemStatusGet`, `SettingsNameGet`,
  `SettingsNameSet`, `AudioPlay`, `WifiStatusGet`, ...), with an **`Async` suffix** — the one
  deliberate deviation from the TS names, since it's the standing .NET convention for
  Task-returning methods and every method here is genuinely async.
- **Organization**: a single `partial class BusyBar`, split one file per namespace
  (`BusyBar.Display.cs`, `BusyBar.System.cs`, `BusyBar.Settings.cs`, ...), mirroring the TS
  library's per-module method-mixin structure so the two codebases stay easy to cross-reference.
- **Param/result types**: C# records generated from the OpenAPI schema, named identically to
  their TS counterparts (`DisplayDrawParams`, `SuccessResponse`, `StatusDevice`,
  `DisplayBrightnessInfo`, `BusySnapshot`, etc.).
- **Request options**: every method takes an optional trailing `RequestOptions` with
  `TimeSpan? Timeout` and `CancellationToken CancellationToken` — the idiomatic .NET replacement
  for TS's `{ timeout, signal }` / `AbortController` pattern.
- **Binary/streaming endpoints** (`POST /busybar/assets/upload`, `GET|POST /busybar/storage/read|write`,
  `GET /busybar/screen`, `POST /busybar/log_dump`) take/return `Stream` in place of TS's
  `Blob`/`ArrayBuffer`.
- Full endpoint-to-method map is derived directly from the OpenAPI document's paths (see
  Appendix in the implementation plan, not duplicated here to avoid drift from the source spec).

## Error handling

Mirrors the four error cases the TS README documents explicitly:

| TS behavior | .NET equivalent |
|---|---|
| HTTP 4xx/5xx, custom error with `status`/`statusText`/`body` | `BusyBarApiException` — carries `StatusCode`, `ReasonPhrase`, parsed error body |
| Timeout (`DOMException`, `name === 'TimeoutError'`) | `TimeoutException`, raised when the effective per-call timeout elapses |
| Aborted via caller signal (`DOMException`, `name === 'AbortError'`) | `OperationCanceledException`, raised when the caller's `CancellationToken` fires |
| Unreachable device / network failure (raw `fetch` `TypeError`) | Unwrapped `HttpRequestException` |

Internally, the per-call timeout and the caller's `CancellationToken` are combined via a linked
`CancellationTokenSource` so both paths funnel through the same cancellation mechanism, then are
distinguished when translating to the exception types above.

## Testing

- xUnit, using a fake `HttpMessageHandler` to unit-test request shaping (path, headers, JSON
  body) and response deserialization per namespace, without needing a real device.
- A small number of integration-style tests, gated behind an xUnit trait/category, that only run
  when a `BUSYBAR_TEST_ADDR` environment variable is set — for opt-in runs against a real BUSY
  Bar or the local emulator referenced by the `busybar-apps` community repo.
- Every generated param/result type gets at least a round-trip serialization test.

## Open questions / assumptions carried into implementation

1. **Spec version mismatch**: the fetched OpenAPI document was requested as
   `openapi.yaml?Name=1.1.1-rc` but reports `api_semver`/version `25.0.0` internally. The
   implementation plan should re-fetch and pin an exact spec version (vendoring the YAML file
   into the repo for reproducibility) rather than relying on the live URL at build time.
2. **`FlashAsync`-style convenience helpers**: the raw API has no native "flash" endpoint —
   only `led_notification_color` on `DisplayDraw`. This library intentionally exposes only the
   raw typed API surface (no flashing convenience method); any such behavior belongs in the
   Halo worker (or another consumer), built on top of `DisplayDrawAsync`.
3. **Auth precedence**: when both `Token` and `HttpAccessPassword` are set (shouldn't normally
   happen — they correspond to different transports), the implementation should send whichever
   the caller most recently set and log a warning if both are non-null at request time.
