using Busy.Bar;
using BusyBar.Tests.Internal;
using Xunit;

namespace BusyBar.Tests;

public class SystemTests
{
    private static (Busy.Bar.BusyBar bar, FakeHttpMessageHandler handler) CreateClient()
    {
        var handler = new FakeHttpMessageHandler();
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        return (new Busy.Bar.BusyBar(http, new BusyBarOptions()), handler);
    }

    [Fact]
    public async Task SystemVersionGetAsync_ParsesApiSemver()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"api_semver\":\"25.0.0\"}";

        var info = await bar.SystemVersionGetAsync();

        Assert.Equal("25.0.0", info.ApiSemver);
    }

    [Fact]
    public async Task SystemStatusDeviceGetAsync_ParsesFirmwareSecurityEnum()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"serial_number\":\"abc\",\"usb_mac\":\"0c:fa:22:21:2a:31\",\"otp_valid\":true,\"firmware_security\":\"secure\"}";

        var device = await bar.SystemStatusDeviceGetAsync();

        Assert.Equal(FirmwareSecurity.Secure, device.FirmwareSecurity);
        Assert.Null(device.WifiMac);
    }

    [Fact]
    public async Task SystemStatusPowerGetAsync_ParsesStateEnum()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"state\":\"discharging\",\"battery_charge\":99,\"battery_voltage\":4183,\"battery_current\":-180,\"usb_voltage\":4843}";

        var power = await bar.SystemStatusPowerGetAsync();

        Assert.Equal(PowerState.Discharging, power.State);
        Assert.Equal(-180, power.BatteryCurrent);
    }

    [Fact]
    public async Task SystemLogDumpAsync_OmitsFilenameQuery_WhenNotSpecified_AndParsesResult()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\",\"path\":\"/ext/dump.txt\"}";

        var result = await bar.SystemLogDumpAsync();

        Assert.Equal(string.Empty, handler.LastRequest!.RequestUri!.Query);
        Assert.Equal("/ext/dump.txt", result.Path);
    }
}
