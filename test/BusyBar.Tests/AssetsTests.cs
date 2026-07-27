using Busy.Bar;
using BusyBar.Tests.Internal;
using Xunit;

namespace BusyBar.Tests;

public class AssetsTests
{
    private static (Busy.Bar.BusyBar bar, FakeHttpMessageHandler handler) CreateClient()
    {
        var handler = new FakeHttpMessageHandler();
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        return (new Busy.Bar.BusyBar(http, new BusyBarOptions()), handler);
    }

    [Fact]
    public async Task AssetsUploadAsync_SendsBinaryBodyWithQueryParams()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";
        using var content = new MemoryStream(new byte[] { 9, 9, 9 });

        var result = await bar.AssetsUploadAsync(new AssetsUploadParams("my_app", "data.png"), content);

        Assert.Equal("OK", result.Result);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Contains("application_name=my_app", handler.LastRequest.RequestUri!.Query);
        Assert.Contains("file=data.png", handler.LastRequest.RequestUri.Query);
        Assert.Equal("application/octet-stream", handler.LastRequest.Content!.Headers.ContentType!.MediaType);
    }

    [Fact]
    public async Task AssetsDeleteAsync_SendsDeleteWithApplicationNameQuery()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.AssetsDeleteAsync(new AssetsDeleteParams("my_app"));

        Assert.Equal(HttpMethod.Delete, handler.LastRequest!.Method);
        Assert.Contains("application_name=my_app", handler.LastRequest.RequestUri!.Query);
    }
}
