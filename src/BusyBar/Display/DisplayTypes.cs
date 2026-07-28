using System.Text.Json.Serialization;

namespace Busy.Bar;

/// <summary>Which of the device's (up to two) displays an element or frame targets.</summary>
public enum DisplayTarget
{
    /// <summary>The front display.</summary>
    [JsonStringEnumMemberName("front")] Front,

    /// <summary>The back display, on dual-display devices.</summary>
    [JsonStringEnumMemberName("back")] Back
}

/// <summary>Anchor point of a display element, used together with its X/Y coordinates to position it.</summary>
public enum ElementAlign
{
    /// <summary>Top-left corner.</summary>
    [JsonStringEnumMemberName("top_left")] TopLeft,

    /// <summary>Top edge, horizontally centered.</summary>
    [JsonStringEnumMemberName("top_mid")] TopMid,

    /// <summary>Top-right corner.</summary>
    [JsonStringEnumMemberName("top_right")] TopRight,

    /// <summary>Left edge, vertically centered.</summary>
    [JsonStringEnumMemberName("mid_left")] MidLeft,

    /// <summary>Horizontally and vertically centered.</summary>
    [JsonStringEnumMemberName("center")] Center,

    /// <summary>Right edge, vertically centered.</summary>
    [JsonStringEnumMemberName("mid_right")] MidRight,

    /// <summary>Bottom-left corner.</summary>
    [JsonStringEnumMemberName("bottom_left")] BottomLeft,

    /// <summary>Bottom edge, horizontally centered.</summary>
    [JsonStringEnumMemberName("bottom_mid")] BottomMid,

    /// <summary>Bottom-right corner.</summary>
    [JsonStringEnumMemberName("bottom_right")] BottomRight
}

/// <summary>Bitmap font used to render a <see cref="TextElement"/> or <see cref="CountdownElement"/>.</summary>
public enum TextFont
{
    /// <summary>The smallest available font.</summary>
    [JsonStringEnumMemberName("tiny")] Tiny,

    /// <summary>A small font.</summary>
    [JsonStringEnumMemberName("small")] Small,

    /// <summary>The default-sized font.</summary>
    [JsonStringEnumMemberName("normal")] Normal,

    /// <summary>A narrower variant of the normal font.</summary>
    [JsonStringEnumMemberName("condensed")] Condensed,

    /// <summary>A bold variant of the normal font.</summary>
    [JsonStringEnumMemberName("bold")] Bold,

    /// <summary>A large font.</summary>
    [JsonStringEnumMemberName("large")] Large,

    /// <summary>The largest available font.</summary>
    [JsonStringEnumMemberName("extra_large")] ExtraLarge,

    /// <summary>The device's globally configured default font.</summary>
    [JsonStringEnumMemberName("global")] Global
}

/// <summary>Whether a <see cref="CountdownElement"/> counts down to, or up from, its target timestamp.</summary>
public enum CountdownDirection
{
    /// <summary>Counts down: shows the time remaining until the target timestamp.</summary>
    [JsonStringEnumMemberName("time_left")] TimeLeft,

    /// <summary>Counts up: shows the time elapsed since the target timestamp.</summary>
    [JsonStringEnumMemberName("time_since")] TimeSince
}

/// <summary>When a <see cref="CountdownElement"/> shows an hours position.</summary>
public enum ShowHours
{
    /// <summary>Only shows the hours position once it is non-zero.</summary>
    [JsonStringEnumMemberName("when_non_zero")] WhenNonZero,

    /// <summary>Always shows the hours position, even when it is zero.</summary>
    [JsonStringEnumMemberName("always")] Always
}

/// <summary>Fill style used to paint a <see cref="RectangleElement"/>.</summary>
public enum RectangleFill
{
    /// <summary>No fill; only the border (if any) is drawn.</summary>
    [JsonStringEnumMemberName("none")] None,

    /// <summary>A single solid fill color, taken from the first entry of <see cref="RectangleElement.FillColors"/>.</summary>
    [JsonStringEnumMemberName("solid")] Solid,

    /// <summary>A horizontal gradient between the two entries of <see cref="RectangleElement.FillColors"/>.</summary>
    [JsonStringEnumMemberName("gradient_h")] GradientH,

    /// <summary>A vertical gradient between the two entries of <see cref="RectangleElement.FillColors"/>.</summary>
    [JsonStringEnumMemberName("gradient_v")] GradientV
}

/// <summary>
/// Unlike <see cref="BusyTimerSettings"/>/<see cref="BusySnapshotState"/>/<see cref="StorageListElement"/>,
/// this library only ever serializes <see cref="DisplayElement"/> (as part of an outgoing
/// <see cref="DisplayDrawParams"/>) — no BUSY Bar endpoint returns display elements back to the client, so
/// System.Text.Json's discriminator-must-be-first constraint (which only applies to deserialization) never
/// applies here.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(TextElement), "text")]
[JsonDerivedType(typeof(ImageElement), "image")]
[JsonDerivedType(typeof(AnimationElement), "animation")]
[JsonDerivedType(typeof(CountdownElement), "countdown")]
[JsonDerivedType(typeof(RectangleElement), "rectangle")]
public abstract record DisplayElement
{
    /// <summary>Unique identifier for the element, used to update or remove it later.</summary>
    public required string Id { get; init; }

    /// <summary>Time in seconds the element should remain displayed (0 for no timeout). Mutually exclusive with <see cref="DisplayUntil"/>.</summary>
    public int? Timeout { get; init; }

    /// <summary>Unix timestamp, in seconds, at which the element is hidden. Mutually exclusive with <see cref="Timeout"/>.</summary>
    public string? DisplayUntil { get; init; }

    /// <summary>X coordinate of the element's anchor point, relative to the top-left of the display.</summary>
    public int X { get; init; }

    /// <summary>Y coordinate of the element's anchor point, relative to the top-left of the display.</summary>
    public int Y { get; init; }

    /// <summary>Which display to draw the element on.</summary>
    public DisplayTarget Display { get; init; } = DisplayTarget.Front;

    /// <summary>Anchor point of the element, used together with <see cref="X"/>/<see cref="Y"/> to position it.</summary>
    public ElementAlign? Align { get; init; }
}

/// <summary>A text label to display.</summary>
public sealed record TextElement : DisplayElement
{
    /// <summary>Text content to display. Printable ASCII only, since fonts are bitmap ASCII.</summary>
    public required string Text { get; init; }

    /// <summary>Font to render the text in.</summary>
    public required TextFont Font { get; init; }

    /// <summary>Text color, in #RRGGBBAA format.</summary>
    public string Color { get; init; } = "#FFFFFFFF";

    /// <summary>Width of the text label, in pixels.</summary>
    public int? Width { get; init; }

    /// <summary>Scroll rate, in pixels per minute, for text that doesn't fit within <see cref="Width"/>.</summary>
    public int? ScrollRate { get; init; }

    /// <summary>Delay, in milliseconds, before the scroll animation begins.</summary>
    public int? ScrollStartDelay { get; init; }

    /// <summary>Pause duration, in milliseconds, between successive scroll cycles.</summary>
    public int? ScrollRepeatDelay { get; init; }
}

/// <summary>Exactly one of <see cref="Path"/> or <see cref="StockPath"/> should be set.</summary>
public sealed record ImageElement : DisplayElement
{
    /// <summary>Path to the image file within the application's assets directory.</summary>
    public string? Path { get; init; }

    /// <summary>Name of a stock image file bundled on the device.</summary>
    public string? StockPath { get; init; }

    /// <summary>Opacity of the image, in the range [0, 100].</summary>
    public int Opacity { get; init; } = 100;
}

/// <summary>Exactly one of <see cref="Path"/> or <see cref="StockPath"/> should be set.</summary>
public sealed record AnimationElement : DisplayElement
{
    /// <summary>Path to the animation file within the application's assets directory.</summary>
    public string? Path { get; init; }

    /// <summary>Name of a stock animation file bundled on the device.</summary>
    public string? StockPath { get; init; }

    /// <summary>Whether to loop the requested part of the animation.</summary>
    public bool Loop { get; init; }

    /// <summary>If this element was previously created with a different range and this is true, the previous range finishes playing before this one starts.</summary>
    public bool AwaitPreviousEnd { get; init; }

    /// <summary>Name of the section to play back. Specify <c>"default"</c> to select the entire animation.</summary>
    public string? Section { get; init; }

    /// <summary>Opacity of the animation, in the range [0, 100].</summary>
    public int Opacity { get; init; } = 100;
}

/// <summary>A live countdown (or count-up) timer to display.</summary>
public sealed record CountdownElement : DisplayElement
{
    /// <summary>Seconds-based Unix UTC timestamp, encoded as a numeric string per the API.</summary>
    public required string Timestamp { get; init; }

    /// <summary>Countdown text color, in #RRGGBBAA format.</summary>
    public string Color { get; init; } = "#FFFFFFFF";

    /// <summary>Whether to count down to, or up from, <see cref="Timestamp"/>.</summary>
    public required CountdownDirection Direction { get; init; }

    /// <summary>When to show the hours position.</summary>
    public required ShowHours ShowHours { get; init; }
}

/// <summary>A rectangle, optionally filled and/or bordered, to display.</summary>
public sealed record RectangleElement : DisplayElement
{
    /// <summary>Width of the rectangle, in pixels.</summary>
    public required int Width { get; init; }

    /// <summary>Height of the rectangle, in pixels.</summary>
    public required int Height { get; init; }

    /// <summary>Corner radius of the rectangle, in pixels (0 for sharp corners).</summary>
    public int Radius { get; init; }

    /// <summary>Fill style of the rectangle.</summary>
    public RectangleFill Fill { get; init; } = RectangleFill.None;

    /// <summary>Colors used to fill the rectangle. Provide one color for <see cref="RectangleFill.Solid"/>, or two for a gradient fill.</summary>
    public IReadOnlyList<string> FillColors { get; init; } = new[] { "#FFFFFFFF", "#00000000" };

    /// <summary>Width of the rectangle's border, in pixels (0 for no border).</summary>
    public int BorderWidth { get; init; } = 1;

    /// <summary>Border color, in #RRGGBBAA format.</summary>
    public string BorderColor { get; init; } = "#FFFFFFFF";
}

/// <summary>Parameters for drawing one or more elements on a display.</summary>
public sealed record DisplayDrawParams
{
    /// <summary>Application ID the drawn elements are attributed to.</summary>
    public required string ApplicationName { get; init; }

    /// <summary>
    /// Draw priority, in the range [1, 100]. A draw request is accepted only when its priority is greater than
    /// or equal to that of the currently running system app; an equal-priority request from a different
    /// <see cref="ApplicationName"/> overrides whatever is currently on screen. Built-in system priority levels:
    /// stub/poweroff apps use 0 (always preemptable, but not settable here), any standard built-in app uses 10,
    /// and an active BUSY/CUSTOM work session uses 90.
    /// </summary>
    public int Priority { get; init; } = 50;

    /// <summary>Color to blink the status LED, in #RRGGBBAA format. If not specified, the LED does not blink.</summary>
    public string? LedNotificationColor { get; init; }

    /// <summary>Elements to draw.</summary>
    public required IReadOnlyList<DisplayElement> Elements { get; init; }
}

/// <summary>Selects which application's display elements to clear.</summary>
/// <param name="ApplicationName">Application ID whose elements should be cleared. If <see langword="null"/>, all elements drawn by the Canvas application are cleared.</param>
public sealed record DisplayClearParams(string? ApplicationName = null);

/// <summary>Selects which display to capture a frame from.</summary>
/// <param name="Display">Display to capture: 0 for front, 1 for back.</param>
public sealed record ScreenFrameGetParams(int Display);

/// <summary>The device's current display brightness.</summary>
/// <param name="Value">Brightness value: either <c>"auto"</c> or a number from 0 to 100.</param>
public sealed record DisplayBrightnessInfo(string Value);

/// <summary>Requested display brightness.</summary>
/// <param name="Value">Brightness value: either <c>"auto"</c> or a number from 0 to 100.</param>
public sealed record DisplayBrightnessParams(string Value);
