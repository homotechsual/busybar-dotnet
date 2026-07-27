using Busy.Bar;
using BusyBar.Tests.Internal;
using Xunit;

namespace BusyBar.Tests;

public class DisplayTests
{
    private static (Busy.Bar.BusyBar bar, FakeHttpMessageHandler handler) CreateClient()
    {
        var handler = new FakeHttpMessageHandler();
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        return (new Busy.Bar.BusyBar(http, new BusyBarOptions()), handler);
    }

    [Fact]
    public async Task DisplayDrawAsync_SerializesTextElementWithDiscriminatorAndDefaults()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";
        var parameters = new DisplayDrawParams
        {
            ApplicationName = "my_app",
            LedNotificationColor = "#FF0000FF",
            Elements = new DisplayElement[]
            {
                new TextElement { Id = "0", Text = "Hello", Font = TextFont.Normal, Align = ElementAlign.Center, Timeout = 10 }
            }
        };

        await bar.DisplayDrawAsync(parameters);

        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Contains("\"type\":\"text\"", handler.LastRequestBody);
        Assert.Contains("\"font\":\"normal\"", handler.LastRequestBody);
        Assert.Contains("\"align\":\"center\"", handler.LastRequestBody);
        Assert.Contains("\"led_notification_color\":\"#FF0000FF\"", handler.LastRequestBody);
        Assert.Contains("\"priority\":50", handler.LastRequestBody);
    }

    [Fact]
    public async Task DisplayClearAsync_OmitsQueryParam_WhenApplicationNameNotSpecified()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.DisplayClearAsync();

        Assert.Equal(HttpMethod.Delete, handler.LastRequest!.Method);
        Assert.Equal(string.Empty, handler.LastRequest.RequestUri!.Query);
    }

    [Fact]
    public async Task DisplayClearAsync_IncludesApplicationName_WhenSpecified()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.DisplayClearAsync(new DisplayClearParams("my_app"));

        Assert.Contains("application_name=my_app", handler.LastRequest!.RequestUri!.Query);
    }

    [Fact]
    public async Task DisplayScreenFrameGetAsync_SendsDisplayQueryParam()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "bmp-bytes";
        handler.ResponseContentType = "image/bmp";

        await using var stream = await bar.DisplayScreenFrameGetAsync(new ScreenFrameGetParams(0));

        Assert.Contains("display=0", handler.LastRequest!.RequestUri!.Query);
        using var reader = new StreamReader(stream);
        Assert.Equal("bmp-bytes", await reader.ReadToEndAsync());
    }

    [Fact]
    public async Task DisplayBrightnessGetAsync_ParsesValue()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"value\":\"auto\"}";

        var info = await bar.DisplayBrightnessGetAsync();

        Assert.Equal("auto", info.Value);
    }

    [Fact]
    public async Task DisplayBrightnessSetAsync_SendsValueQueryParam()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.DisplayBrightnessSetAsync(new DisplayBrightnessParams("50"));

        Assert.Contains("value=50", handler.LastRequest!.RequestUri!.Query);
    }
}
