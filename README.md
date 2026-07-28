# BusyBar

[![CI](https://img.shields.io/github/actions/workflow/status/homotechsual/busybar-dotnet/ci.yml?branch=main&style=for-the-badge&label=CI)](https://github.com/homotechsual/busybar-dotnet/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/BusyBar?style=for-the-badge&label=NuGet)](https://www.nuget.org/packages/BusyBar)

A typed .NET client for the [BUSY Bar HTTP API](https://docs.busy.app/bar/dev/http-api),
modelled on the official [`@busy-app/busy-lib`](https://github.com/busy-app/busylib-ts)
TypeScript client.

## Install

```bash
dotnet add package BusyBar
```

## Quick start

```csharp
using Busy.Bar;

var bar = new BusyBar(new BusyBarOptions { Addr = "10.0.4.20" });

var status = await bar.SystemStatusGetAsync();

await bar.DisplayDrawAsync(new DisplayDrawParams
{
    ApplicationName = "my_app",
    Elements = new DisplayElement[]
    {
        new TextElement { Id = "0", Text = "Hello!", Font = TextFont.Normal, Align = ElementAlign.Center }
    }
});
```

## Scope

Covers the full HTTP API surface: Account, Assets, Audio, Ble, Busy (timer), Display, Input,
Settings, SmartHome, Storage, System, Time, Update, and Wifi. Real-time WebSocket state
streaming (`StateStream` in the TS library) and WebGL rendering (`ScreenRenderer`) are out of
scope — this library covers the HTTP API only.

## Error handling

- Non-2xx response → `BusyBarApiException` (`StatusCode`, `ReasonPhrase`, `RawBody`, `ErrorBody`)
- Per-call timeout elapses → `TimeoutException`
- Caller-supplied `CancellationToken` fires → `OperationCanceledException`
- Device unreachable / network failure → `HttpRequestException`

## License

MIT
