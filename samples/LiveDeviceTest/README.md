# LiveDeviceTest

A small interactive console sample that drives a real BUSY Bar device using the `BusyBar` library,
built for hands-on validation while watching the physical device. Not an automated test — every
action prints what to expect and leaves it to you to confirm on the display.

## Usage

```bash
# Defaults to 10.0.4.20 (USB); override with BUSYBAR_TEST_ADDR
dotnet run --project samples/LiveDeviceTest -- <action> [args]
```

| Action | What it does |
|---|---|
| `status` | Prints device name, serial, firmware, and current brightness |
| `clear` | Clears the display |
| `draw-hello` | Draws centered green "Hello!" text |
| `draw-multi` | Draws a filled rounded rectangle with white "HELLO" text on top |
| `scroll` | Draws a scrolling orange text banner |
| `countdown` | Draws a 30-second countdown timer |
| `led` | Draws text and sets the status LED to red via `led_notification_color` |
| `brightness <value>` | Sets display brightness (`0`-`100` or `auto`) |
| `scroll-diag [frameCount] [intervalMs]` | Captures successive screen frames via `DisplayScreenFrameGetAsync`, hashing each to objectively show whether a scroll animation is actually progressing — see below |

## Findings from live validation against real hardware

- **`DisplayDrawAsync` is additive, not a canvas replace.** Elements from a previous draw call
  stay on screen (keyed by their own `Id` within an `ApplicationName`) until you either reuse the
  same `Id` to update them in place, or call `DisplayClearAsync`. Confirmed live: without an
  explicit clear between demo steps, an earlier rectangle stayed visible underneath later text.
  Every `draw-*` action here clears first.
- **`Align` anchors the element's own bounding box at `(X, Y)` — it does not center the element
  within the canvas.** The BUSY Bar's display is 72x16px. Left at the default `X=0, Y=0`, an
  `Align.Center` element gets its center pinned to the canvas's top-left corner, pushing roughly
  half of it off-screen. To actually center something, set `X`/`Y` to the canvas's own center
  point (36, 8) — this matches the vendored OpenAPI spec's own example, which was easy to miss.
- **Scroll pacing is easy to misread as "stuck."** `ScrollRepeatDelay` briefly holds the display on
  the same (readable) opening text at the start of every loop, which reads as a pause even when
  the animation is genuinely running continuously. Confirmed via `scroll-diag`: hashing captured
  frames from `DisplayScreenFrameGetAsync` showed every sampled frame differing from its
  predecessor, with the loop returning to an identical frame ~4.4s later — proof of continuous,
  correctly-looping motion, not a stall.
- **Display brightness works correctly** (verified via round-trip get/set and a visible max-vs-min
  comparison), but small changes (e.g. `10` vs whatever `auto` happened to be choosing under
  ambient light) can be hard to perceive — use the extremes (`0`/`100`) to sanity-check.
