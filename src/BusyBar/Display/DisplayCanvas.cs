namespace Busy.Bar;

/// <summary>
/// Convenience helpers for positioning <see cref="DisplayElement"/>s on the BUSY Bar's screen. Not
/// part of the raw HTTP API — <see cref="DisplayElement.Align"/> only anchors an element's own
/// bounding box at <see cref="DisplayElement.X"/>/<see cref="DisplayElement.Y"/>; it does not center
/// the element within the canvas the way you might expect. Confirmed against physical hardware:
/// left at the default <c>X=0, Y=0</c>, an <see cref="ElementAlign.Center"/> element gets its center
/// pinned to the canvas's top-left corner, pushing roughly half of it off-screen. Use
/// <see cref="AnchorFor"/> to compute the correct <c>X</c>/<c>Y</c> for a given alignment against the
/// canvas's own bounds.
/// </summary>
public static class DisplayCanvas
{
    /// <summary>Width, in pixels, of the standard BUSY Bar display (model BB.1 — the only model
    /// currently documented; revisit if a device with different dimensions ships).</summary>
    public const int Width = 72;

    /// <summary>Height, in pixels, of the standard BUSY Bar display (model BB.1 — the only model
    /// currently documented; revisit if a device with different dimensions ships).</summary>
    public const int Height = 16;

    /// <summary>The canvas's own center point.</summary>
    public static (int X, int Y) Center => (Width / 2, Height / 2);

    /// <summary>
    /// Returns the (X, Y) coordinate on the canvas that corresponds to the given alignment — e.g.
    /// <see cref="ElementAlign.Center"/> returns the canvas's exact center, <see cref="ElementAlign.TopRight"/>
    /// its top-right corner. Assign the result to a <see cref="DisplayElement"/>'s <see cref="DisplayElement.X"/>/
    /// <see cref="DisplayElement.Y"/> so the same alignment anchors it relative to the whole canvas rather than
    /// to the canvas's top-left corner.
    /// </summary>
    public static (int X, int Y) AnchorFor(ElementAlign align) => align switch
    {
        ElementAlign.TopLeft => (0, 0),
        ElementAlign.TopMid => (Width / 2, 0),
        ElementAlign.TopRight => (Width, 0),
        ElementAlign.MidLeft => (0, Height / 2),
        ElementAlign.Center => (Width / 2, Height / 2),
        ElementAlign.MidRight => (Width, Height / 2),
        ElementAlign.BottomLeft => (0, Height),
        ElementAlign.BottomMid => (Width / 2, Height),
        ElementAlign.BottomRight => (Width, Height),
        _ => throw new ArgumentOutOfRangeException(nameof(align), align, "Unknown ElementAlign value.")
    };
}
