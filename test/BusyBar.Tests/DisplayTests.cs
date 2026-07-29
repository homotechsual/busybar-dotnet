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
    public async Task DisplayDrawAsync_SerializesImageElementWithDiscriminator()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";
        var parameters = new DisplayDrawParams
        {
            ApplicationName = "my_app",
            Elements = new DisplayElement[]
            {
                new ImageElement { Id = "0", StockPath = "logo.png", Opacity = 80 }
            }
        };

        await bar.DisplayDrawAsync(parameters);

        Assert.Contains("\"type\":\"image\"", handler.LastRequestBody);
        Assert.Contains("\"stock_path\":\"logo.png\"", handler.LastRequestBody);
        Assert.Contains("\"opacity\":80", handler.LastRequestBody);
    }

    [Fact]
    public async Task DisplayDrawAsync_SerializesAnimationElementWithDiscriminator()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";
        var parameters = new DisplayDrawParams
        {
            ApplicationName = "my_app",
            Elements = new DisplayElement[]
            {
                new AnimationElement { Id = "0", StockPath = "wave.gif", Loop = true, Section = "default" }
            }
        };

        await bar.DisplayDrawAsync(parameters);

        Assert.Contains("\"type\":\"animation\"", handler.LastRequestBody);
        Assert.Contains("\"loop\":true", handler.LastRequestBody);
        Assert.Contains("\"section\":\"default\"", handler.LastRequestBody);
    }

    [Fact]
    public async Task DisplayDrawAsync_SerializesCountdownElementWithDiscriminator()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";
        var parameters = new DisplayDrawParams
        {
            ApplicationName = "my_app",
            Elements = new DisplayElement[]
            {
                new CountdownElement
                {
                    Id = "0",
                    Timestamp = "1761582532",
                    Direction = CountdownDirection.TimeLeft,
                    ShowHours = ShowHours.Always
                }
            }
        };

        await bar.DisplayDrawAsync(parameters);

        Assert.Contains("\"type\":\"countdown\"", handler.LastRequestBody);
        Assert.Contains("\"direction\":\"time_left\"", handler.LastRequestBody);
        Assert.Contains("\"show_hours\":\"always\"", handler.LastRequestBody);
    }

    [Fact]
    public async Task DisplayDrawAsync_SerializesRectangleElementWithDiscriminator()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";
        var parameters = new DisplayDrawParams
        {
            ApplicationName = "my_app",
            Elements = new DisplayElement[]
            {
                new RectangleElement { Id = "0", Width = 20, Height = 10, Fill = RectangleFill.Solid }
            }
        };

        await bar.DisplayDrawAsync(parameters);

        Assert.Contains("\"type\":\"rectangle\"", handler.LastRequestBody);
        Assert.Contains("\"fill\":\"solid\"", handler.LastRequestBody);
        Assert.Contains("\"width\":20", handler.LastRequestBody);
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
