using Busy.Bar;
using BusyBar.Tests.Internal;
using Xunit;

namespace BusyBar.Tests;

public class WifiTests
{
    private static (Busy.Bar.BusyBar bar, FakeHttpMessageHandler handler) CreateClient()
    {
        var handler = new FakeHttpMessageHandler();
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        return (new Busy.Bar.BusyBar(http, new BusyBarOptions()), handler);
    }

    [Fact]
    public async Task WifiStatusGetAsync_ParsesDisconnectedStateWithoutOptionalFields()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"state\":\"disconnected\"}";

        var status = await bar.WifiStatusGetAsync();

        Assert.Equal(WifiConnState.Disconnected, status.State);
        Assert.Null(status.Ssid);
    }

    [Fact]
    public async Task WifiStatusGetAsync_ParsesConnectedStateWithSecurityLiteralCasing()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = """
        {"state":"connected","ssid":"Your_WIFI_SSID","bssid":"EC:5A:00:0B:55:1D","channel":3,"rssi":-43,"security":"WPA2/WPA3","ip_config":{"ip_method":"dhcp","ip_type":"ipv4","address":"192.168.50.5"}}
        """;

        var status = await bar.WifiStatusGetAsync();

        Assert.Equal(WifiSecurityMethod.Wpa2Wpa3, status.Security);
        Assert.Equal(WifiIpType.Ipv4, status.IpConfig!.IpType);
        Assert.Equal("192.168.50.5", status.IpConfig.Address);
    }
}
