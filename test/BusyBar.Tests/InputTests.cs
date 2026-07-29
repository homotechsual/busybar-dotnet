using Busy.Bar;
using BusyBar.Tests.Internal;
using Xunit;

namespace BusyBar.Tests;

public class InputTests
{
    [Fact]
    public async Task InputKeySetAsync_SendsKeyQueryParam()
    {
        var handler = new FakeHttpMessageHandler { ResponseBody = "{\"result\":\"OK\"}" };
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        var bar = new Busy.Bar.BusyBar(http, new BusyBarOptions());

        await bar.InputKeySetAsync(new InputKeyParams(InputKey.Off));

        Assert.Contains("key=off", handler.LastRequest!.RequestUri!.Query);
    }

    [Theory]
    [InlineData(InputKey.Up, "up")]
    [InlineData(InputKey.Down, "down")]
    [InlineData(InputKey.Ok, "ok")]
    [InlineData(InputKey.Back, "back")]
    [InlineData(InputKey.Start, "start")]
    [InlineData(InputKey.Busy, "busy")]
    [InlineData(InputKey.Custom, "custom")]
    [InlineData(InputKey.Off, "off")]
    [InlineData(InputKey.Apps, "apps")]
    [InlineData(InputKey.Settings, "settings")]
    public async Task InputKeySetAsync_MapsEveryKeyToItsApiString(InputKey key, string expected)
    {
        var handler = new FakeHttpMessageHandler { ResponseBody = "{\"result\":\"OK\"}" };
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        var bar = new Busy.Bar.BusyBar(http, new BusyBarOptions());

        await bar.InputKeySetAsync(new InputKeyParams(key));

        Assert.Contains($"key={expected}", handler.LastRequest!.RequestUri!.Query);
    }

    [Fact]
    public async Task InputKeySetAsync_ThrowsArgumentOutOfRangeException_ForUndefinedKey()
    {
        var handler = new FakeHttpMessageHandler { ResponseBody = "{\"result\":\"OK\"}" };
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        var bar = new Busy.Bar.BusyBar(http, new BusyBarOptions());

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => bar.InputKeySetAsync(new InputKeyParams((InputKey)999)));
    }
}
