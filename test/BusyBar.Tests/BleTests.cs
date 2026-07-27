using Busy.Bar;
using BusyBar.Tests.Internal;
using Xunit;

namespace BusyBar.Tests;

public class BleTests
{
    private static (Busy.Bar.BusyBar bar, FakeHttpMessageHandler handler) CreateClient()
    {
        var handler = new FakeHttpMessageHandler();
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        return (new Busy.Bar.BusyBar(http, new BusyBarOptions()), handler);
    }

    [Fact]
    public async Task BleStatusGetAsync_ParsesInternalErrorEnumValueWithSpace()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"status\":\"internal error\"}";

        var status = await bar.BleStatusGetAsync();

        Assert.Equal(BleStatus.InternalError, status.Status);
    }

    [Fact]
    public async Task BleStatusGetAsync_ParsesConnectedWithAddress()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"status\":\"connected\",\"address\":\"50:DA:D6:FE:DD:A9\"}";

        var status = await bar.BleStatusGetAsync();

        Assert.Equal(BleStatus.Connected, status.Status);
        Assert.Equal("50:DA:D6:FE:DD:A9", status.Address);
    }

    [Fact]
    public async Task BlePairingRemoveAsync_SendsDelete()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.BlePairingRemoveAsync();

        Assert.Equal(HttpMethod.Delete, handler.LastRequest!.Method);
        Assert.EndsWith("busybar/ble/pairing", handler.LastRequest.RequestUri!.ToString());
    }
}
