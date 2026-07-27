using Busy.Bar;
using BusyBar.Tests.Internal;
using Xunit;

namespace BusyBar.Tests;

public class AccountTests
{
    private static (Busy.Bar.BusyBar bar, FakeHttpMessageHandler handler) CreateClient()
    {
        var handler = new FakeHttpMessageHandler();
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        return (new Busy.Bar.BusyBar(http, new BusyBarOptions()), handler);
    }

    [Fact]
    public async Task AccountInfoGetAsync_GetsExpectedPathAndParsesResponse()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"linked\":true,\"id\":\"abc\",\"email\":\"a@b.com\",\"user_id\":\"u1\"}";

        var info = await bar.AccountInfoGetAsync();

        Assert.EndsWith("busybar/account/info", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.True(info.Linked);
        Assert.Equal("a@b.com", info.Email);
    }

    [Fact]
    public async Task AccountStatusGetAsync_ParsesConnectionStatusEnum()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"status\":\"connected\"}";

        var status = await bar.AccountStatusGetAsync();

        Assert.Equal(AccountConnectionStatus.Connected, status.Status);
    }

    [Fact]
    public async Task AccountBackendGetAsync_ParsesRequiredFields()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"server_url\":\"default\",\"client_cert_type\":\"custom\",\"ignore_server_cert\":false}";

        var backend = await bar.AccountBackendGetAsync();

        Assert.EndsWith("busybar/account/backend", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("default", backend.ServerUrl);
        Assert.Equal(ClientCertType.Custom, backend.ClientCertType);
    }
}
