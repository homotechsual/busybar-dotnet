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
}
