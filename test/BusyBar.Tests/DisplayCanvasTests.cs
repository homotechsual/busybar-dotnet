using Busy.Bar;
using Xunit;

namespace BusyBar.Tests;

public class DisplayCanvasTests
{
    [Fact]
    public void Center_IsHalfOfWidthAndHeight()
    {
        Assert.Equal((36, 8), DisplayCanvas.Center);
    }

    [Theory]
    [InlineData(ElementAlign.TopLeft, 0, 0)]
    [InlineData(ElementAlign.TopMid, 36, 0)]
    [InlineData(ElementAlign.TopRight, 72, 0)]
    [InlineData(ElementAlign.MidLeft, 0, 8)]
    [InlineData(ElementAlign.Center, 36, 8)]
    [InlineData(ElementAlign.MidRight, 72, 8)]
    [InlineData(ElementAlign.BottomLeft, 0, 16)]
    [InlineData(ElementAlign.BottomMid, 36, 16)]
    [InlineData(ElementAlign.BottomRight, 72, 16)]
    public void AnchorFor_ReturnsCorrectCoordinateForEveryAlignment(ElementAlign align, int expectedX, int expectedY)
    {
        var (x, y) = DisplayCanvas.AnchorFor(align);

        Assert.Equal(expectedX, x);
        Assert.Equal(expectedY, y);
    }

    [Fact]
    public void AnchorFor_Center_MatchesCenterProperty()
    {
        Assert.Equal(DisplayCanvas.Center, DisplayCanvas.AnchorFor(ElementAlign.Center));
    }

    [Fact]
    public void AnchorFor_ThrowsArgumentOutOfRangeException_ForUndefinedAlignment()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => DisplayCanvas.AnchorFor((ElementAlign)999));
    }
}
