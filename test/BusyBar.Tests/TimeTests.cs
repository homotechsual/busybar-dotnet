using Busy.Bar;
using BusyBar.Tests.Internal;
using Xunit;

namespace BusyBar.Tests;

public class TimeTests
{
    private static (Busy.Bar.BusyBar bar, FakeHttpMessageHandler handler) CreateClient()
    {
        var handler = new FakeHttpMessageHandler();
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        return (new Busy.Bar.BusyBar(http, new BusyBarOptions()), handler);
    }

    [Fact]
    public async Task TimeGetAsync_ParsesTimestamp()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"timestamp\":\"2025-10-02T14:30:45+04:00\"}";

        var info = await bar.TimeGetAsync();

        Assert.Equal("2025-10-02T14:30:45+04:00", info.Timestamp);
    }

    [Fact]
    public async Task TimeSetTimestampAsync_SendsTimestampQueryParam()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.TimeSetTimestampAsync(new TimeSetTimestampParams("2025-10-02T14:30:45+0100"));

        Assert.Contains("timestamp=2025-10-02T14%3A30%3A45%2B0100", handler.LastRequest!.RequestUri!.Query);
    }

    [Fact]
    public async Task TimeTimezoneGetAsync_ParsesTimezoneInfo()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"name\":\"Bangalore\",\"offset\":\"+05:30\",\"abbr\":\"IST\"}";

        var info = await bar.TimeTimezoneGetAsync();

        Assert.Equal("Bangalore", info.Name);
        Assert.Equal("IST", info.Abbr);
    }

    [Fact]
    public async Task TimeTimezoneSetAsync_SendsTimezoneQueryParam()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.TimeTimezoneSetAsync(new TimeSetTimezoneParams("Bangalore"));

        Assert.Contains("timezone=Bangalore", handler.LastRequest!.RequestUri!.Query);
    }

    [Fact]
    public async Task TimeTzlistGetAsync_ParsesListOfTimezones()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"list\":[{\"name\":\"Bangalore\",\"offset\":\"+05:30\",\"abbr\":\"IST\"}]}";

        var list = await bar.TimeTzlistGetAsync();

        Assert.Single(list.List);
        Assert.Equal("Bangalore", list.List[0].Name);
    }
}
