using Busy.Bar;
using BusyBar.Tests.Internal;
using Xunit;

namespace BusyBar.Tests;

public class AudioTests
{
    private static (Busy.Bar.BusyBar bar, FakeHttpMessageHandler handler) CreateClient()
    {
        var handler = new FakeHttpMessageHandler();
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        return (new Busy.Bar.BusyBar(http, new BusyBarOptions()), handler);
    }

    [Fact]
    public async Task AudioPlayAsync_SendsJsonBodyWithStockPath()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.AudioPlayAsync(new AudioPlayParams("my_app", StockPath: "shared/beep.snd"));

        Assert.Contains("\"stock_path\":\"shared/beep.snd\"", handler.LastRequestBody);
        Assert.DoesNotContain("\"path\"", handler.LastRequestBody);
    }

    [Fact]
    public async Task AudioStopAsync_SendsDeleteWithNoBody()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.AudioStopAsync();

        Assert.Equal(HttpMethod.Delete, handler.LastRequest!.Method);
        Assert.Null(handler.LastRequest.Content);
    }

    [Fact]
    public async Task AudioVolumeGetAsync_ParsesVolume()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"volume\":50}";

        var info = await bar.AudioVolumeGetAsync();

        Assert.Equal(50, info.Volume);
    }

    [Fact]
    public async Task AudioVolumeSetAsync_SendsVolumeAndSilentQueryParams()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.AudioVolumeSetAsync(new AudioVolumeSetParams(75, Silent: 1));

        Assert.Contains("volume=75", handler.LastRequest!.RequestUri!.Query);
        Assert.Contains("silent=1", handler.LastRequest.RequestUri.Query);
    }
}
