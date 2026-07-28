using System.Text.Json.Serialization;

namespace Busy.Bar;

public enum DisplayTarget
{
    [JsonStringEnumMemberName("front")] Front,
    [JsonStringEnumMemberName("back")] Back
}

public enum ElementAlign
{
    [JsonStringEnumMemberName("top_left")] TopLeft,
    [JsonStringEnumMemberName("top_mid")] TopMid,
    [JsonStringEnumMemberName("top_right")] TopRight,
    [JsonStringEnumMemberName("mid_left")] MidLeft,
    [JsonStringEnumMemberName("center")] Center,
    [JsonStringEnumMemberName("mid_right")] MidRight,
    [JsonStringEnumMemberName("bottom_left")] BottomLeft,
    [JsonStringEnumMemberName("bottom_mid")] BottomMid,
    [JsonStringEnumMemberName("bottom_right")] BottomRight
}

public enum TextFont
{
    [JsonStringEnumMemberName("tiny")] Tiny,
    [JsonStringEnumMemberName("small")] Small,
    [JsonStringEnumMemberName("normal")] Normal,
    [JsonStringEnumMemberName("condensed")] Condensed,
    [JsonStringEnumMemberName("bold")] Bold,
    [JsonStringEnumMemberName("large")] Large,
    [JsonStringEnumMemberName("extra_large")] ExtraLarge,
    [JsonStringEnumMemberName("global")] Global
}

public enum CountdownDirection
{
    [JsonStringEnumMemberName("time_left")] TimeLeft,
    [JsonStringEnumMemberName("time_since")] TimeSince
}

public enum ShowHours
{
    [JsonStringEnumMemberName("when_non_zero")] WhenNonZero,
    [JsonStringEnumMemberName("always")] Always
}

public enum RectangleFill
{
    [JsonStringEnumMemberName("none")] None,
    [JsonStringEnumMemberName("solid")] Solid,
    [JsonStringEnumMemberName("gradient_h")] GradientH,
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
    public required string Id { get; init; }
    public int? Timeout { get; init; }
    public string? DisplayUntil { get; init; }
    public int X { get; init; }
    public int Y { get; init; }
    public DisplayTarget Display { get; init; } = DisplayTarget.Front;
    public ElementAlign? Align { get; init; }
}

public sealed record TextElement : DisplayElement
{
    public required string Text { get; init; }
    public required TextFont Font { get; init; }
    public string Color { get; init; } = "#FFFFFFFF";
    public int? Width { get; init; }
    public int? ScrollRate { get; init; }
    public int? ScrollStartDelay { get; init; }
    public int? ScrollRepeatDelay { get; init; }
}

/// <summary>Exactly one of <see cref="Path"/> or <see cref="StockPath"/> should be set.</summary>
public sealed record ImageElement : DisplayElement
{
    public string? Path { get; init; }
    public string? StockPath { get; init; }
    public int Opacity { get; init; } = 100;
}

/// <summary>Exactly one of <see cref="Path"/> or <see cref="StockPath"/> should be set.</summary>
public sealed record AnimationElement : DisplayElement
{
    public string? Path { get; init; }
    public string? StockPath { get; init; }
    public bool Loop { get; init; }
    public bool AwaitPreviousEnd { get; init; }
    public string? Section { get; init; }
    public int Opacity { get; init; } = 100;
}

public sealed record CountdownElement : DisplayElement
{
    /// <summary>Seconds-based Unix UTC timestamp, encoded as a numeric string per the API.</summary>
    public required string Timestamp { get; init; }
    public string Color { get; init; } = "#FFFFFFFF";
    public required CountdownDirection Direction { get; init; }
    public required ShowHours ShowHours { get; init; }
}

public sealed record RectangleElement : DisplayElement
{
    public required int Width { get; init; }
    public required int Height { get; init; }
    public int Radius { get; init; }
    public RectangleFill Fill { get; init; } = RectangleFill.None;
    public IReadOnlyList<string> FillColors { get; init; } = new[] { "#FFFFFFFF", "#00000000" };
    public int BorderWidth { get; init; } = 1;
    public string BorderColor { get; init; } = "#FFFFFFFF";
}

public sealed record DisplayDrawParams
{
    public required string ApplicationName { get; init; }
    public int Priority { get; init; } = 50;
    public string? LedNotificationColor { get; init; }
    public required IReadOnlyList<DisplayElement> Elements { get; init; }
}

public sealed record DisplayClearParams(string? ApplicationName = null);

public sealed record ScreenFrameGetParams(int Display);

public sealed record DisplayBrightnessInfo(string Value);

public sealed record DisplayBrightnessParams(string Value);
