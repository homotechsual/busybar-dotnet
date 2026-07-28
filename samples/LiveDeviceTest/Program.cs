using Busy.Bar;

// The BUSY Bar's LED matrix is 72x16 pixels. `Align` anchors the ELEMENT's own bounding box
// (its center, an edge, or a corner, depending on the align value) at (X, Y) — it does NOT
// automatically center the element within the canvas. To actually center something on screen,
// X/Y must be set to the canvas's own center point. Left at their default of (0, 0), an
// Align.Center element gets its center pinned to the canvas's top-left corner, pushing roughly
// half of it off-screen up and to the left. (Confirmed live: this is exactly what happened when
// this sample first shipped without CanvasCenterX/Y below.)
const int CanvasWidth = 72;
const int CanvasHeight = 16;
const int CanvasCenterX = CanvasWidth / 2;
const int CanvasCenterY = CanvasHeight / 2;

// DisplayDrawAsync is ADDITIVE, not a canvas replace: elements from a previous draw call persist
// on screen (keyed by their own Id within an ApplicationName) until you either reuse the same Id
// to update them in place, or call DisplayClearAsync to remove them. Confirmed live: without an
// explicit clear between demo steps below, an earlier step's rectangle stayed visible underneath
// later steps' text. Each draw-* action here clears first so it starts from a blank canvas.

var addr = Environment.GetEnvironmentVariable("BUSYBAR_TEST_ADDR") ?? "10.0.4.20";
var action = args.Length > 0 ? args[0] : "status";

using var bar = new BusyBar(new BusyBarOptions { Addr = addr });

Console.WriteLine($"Connecting to BusyBar at {addr} ...");

switch (action)
{
    case "status":
        var status = await bar.SystemStatusGetAsync();
        var name = await bar.NameGetAsync();
        var brightness = await bar.DisplayBrightnessGetAsync();
        Console.WriteLine($"Name: {name.Name}");
        Console.WriteLine($"Serial: {status.Device?.SerialNumber}");
        Console.WriteLine($"Firmware: {status.Firmware?.Version} (api_semver {status.System?.ApiSemver})");
        Console.WriteLine($"Brightness: {brightness.Value}");
        break;

    case "clear":
        await bar.DisplayClearAsync();
        Console.WriteLine("Display cleared.");
        break;

    case "draw-hello":
        await bar.DisplayClearAsync(); // draw is additive — clear first so this demo starts from a blank canvas
        await bar.DisplayDrawAsync(new DisplayDrawParams
        {
            ApplicationName = "live_test",
            Elements = new DisplayElement[]
            {
                new TextElement
                {
                    Id = "hello",
                    Text = "Hello!",
                    Font = TextFont.Normal,
                    Align = ElementAlign.Center,
                    X = CanvasCenterX,
                    Y = CanvasCenterY,
                    Color = "#00FF00FF",
                }
            }
        });
        Console.WriteLine("Drew centered green text 'Hello!' — check the display.");
        break;

    case "draw-multi":
        await bar.DisplayClearAsync(); // draw is additive — clear first so this demo starts from a blank canvas
        await bar.DisplayDrawAsync(new DisplayDrawParams
        {
            ApplicationName = "live_test",
            Elements = new DisplayElement[]
            {
                new RectangleElement
                {
                    Id = "box",
                    X = 4,
                    Y = 2,
                    Width = 64,
                    Height = 12,
                    Radius = 2,
                    Fill = RectangleFill.Solid,
                    FillColors = new[] { "#0033AAFF" },
                    BorderWidth = 1,
                    BorderColor = "#FFFFFFFF",
                },
                new TextElement
                {
                    Id = "label",
                    Text = "HELLO",
                    Font = TextFont.Small,
                    Align = ElementAlign.Center,
                    X = CanvasCenterX,
                    Y = CanvasCenterY,
                    Color = "#FFFFFFFF",
                }
            }
        });
        Console.WriteLine("Drew a filled blue rounded rectangle with white 'HELLO' text on top — check the display.");
        break;

    case "scroll":
        await bar.DisplayClearAsync(); // draw is additive — clear first so this demo starts from a blank canvas
        await bar.DisplayDrawAsync(new DisplayDrawParams
        {
            ApplicationName = "live_test",
            Elements = new DisplayElement[]
            {
                new TextElement
                {
                    Id = "scroller",
                    Text = "This is a long scrolling message from the BusyBar .NET client library!",
                    Font = TextFont.Normal,
                    Align = ElementAlign.MidLeft,
                    X = 0,
                    Y = CanvasCenterY,
                    Width = CanvasWidth,
                    ScrollRate = 8000,
                    ScrollStartDelay = 300,
                    ScrollRepeatDelay = 300,
                    Color = "#FFAA00FF",
                }
            }
        });
        Console.WriteLine("Drew a scrolling orange text banner — check the display, it should scroll left.");
        break;

    case "countdown":
        await bar.DisplayClearAsync(); // draw is additive — clear first so this demo starts from a blank canvas
        var target = DateTimeOffset.UtcNow.AddSeconds(30).ToUnixTimeSeconds();
        await bar.DisplayDrawAsync(new DisplayDrawParams
        {
            ApplicationName = "live_test",
            Elements = new DisplayElement[]
            {
                new CountdownElement
                {
                    Id = "timer",
                    Timestamp = target.ToString(),
                    Direction = CountdownDirection.TimeLeft,
                    ShowHours = ShowHours.WhenNonZero,
                    Align = ElementAlign.Center,
                    X = CanvasCenterX,
                    Y = CanvasCenterY,
                    Color = "#00FFFFFF",
                }
            }
        });
        Console.WriteLine("Drew a 30-second countdown timer — check the display, it should be counting down.");
        break;

    case "led":
        await bar.DisplayClearAsync(); // draw is additive — clear first so this demo starts from a blank canvas
        await bar.DisplayDrawAsync(new DisplayDrawParams
        {
            ApplicationName = "live_test",
            LedNotificationColor = "#FF0000FF",
            Elements = new DisplayElement[]
            {
                new TextElement
                {
                    Id = "led_label",
                    Text = "LED",
                    Font = TextFont.Normal,
                    Align = ElementAlign.Center,
                    X = CanvasCenterX,
                    Y = CanvasCenterY,
                    Color = "#FFFFFFFF",
                }
            }
        });
        Console.WriteLine("Set led_notification_color to red — check the device's status LED.");
        break;

    case "brightness":
        var value = args.Length > 1 ? args[1] : "50";
        await bar.DisplayBrightnessSetAsync(new DisplayBrightnessParams(value));
        Console.WriteLine($"Set brightness to '{value}'.");
        break;

    case "scroll-diag":
        // Objective diagnostic for the "scroll looks like it pauses" behavior reported watching the
        // device live: capture successive frames from GET /screen during a scroll and hash each one,
        // so we can tell apart "genuinely frozen" (identical hashes throughout) from "moving in large
        // discrete jumps rather than smoothly" (hashes change, but only at a few distinct points) —
        // without relying on more rounds of subjective visual description.
        await bar.DisplayClearAsync();
        await bar.DisplayDrawAsync(new DisplayDrawParams
        {
            ApplicationName = "live_test",
            Elements = new DisplayElement[]
            {
                new TextElement
                {
                    Id = "scroller",
                    Text = "This is a long scrolling message from the BusyBar .NET client library!",
                    Font = TextFont.Normal,
                    Align = ElementAlign.MidLeft,
                    X = 0,
                    Y = CanvasCenterY,
                    Width = CanvasWidth,
                    ScrollRate = 8000,
                    ScrollStartDelay = 300,
                    ScrollRepeatDelay = 300,
                    Color = "#FFAA00FF",
                }
            }
        });

        var frameCount = args.Length > 1 ? int.Parse(args[1]) : 10;
        var frameIntervalMs = args.Length > 2 ? int.Parse(args[2]) : 400;
        string? previousHash = null;
        for (var i = 0; i < frameCount; i++)
        {
            await using var frame = await bar.DisplayScreenFrameGetAsync(new ScreenFrameGetParams(0));
            using var ms = new MemoryStream();
            await frame.CopyToAsync(ms);
            var bytes = ms.ToArray();
            var hash = Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(bytes));
            var changed = previousHash is not null && hash != previousHash ? "CHANGED" : previousHash is null ? "(first)" : "same";
            Console.WriteLine($"[{i,2}] t={i * frameIntervalMs,5}ms  {bytes.Length,5} bytes  sha256={hash[..12]}  {changed}");
            previousHash = hash;
            if (i < frameCount - 1) await Task.Delay(frameIntervalMs);
        }
        break;

    default:
        Console.WriteLine($"Unknown action '{action}'. Valid actions: status, clear, draw-hello, draw-multi, scroll, countdown, led, brightness <value>, scroll-diag [frameCount] [intervalMs]");
        break;
}
