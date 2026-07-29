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
    public async Task SystemTransportGetAsync_ParsesTransportType()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"type\":\"wifi\"}";

        var info = await bar.SystemTransportGetAsync();

        Assert.Equal(NetworkTransportType.Wifi, info.Type);
    }

    [Fact]
    public async Task SystemStatusGetAsync_ParsesNestedSubStatuses()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = """
        {"device":{"serial_number":"abc","usb_mac":"0c:fa:22:21:2a:31","otp_valid":true,"firmware_security":"secure"},"firmware":{"version":"1.0.2","target":1,"branch":"main","build_date":"2026-01-01","commit_hash":"abcdef1"},"system":{"api_semver":"25.0.0","uptime":"1d","boot_time":1761582532,"auto_update_enabled":true},"power":{"state":"discharging","battery_charge":99,"battery_voltage":4183,"battery_current":-180,"usb_voltage":4843}}
        """;

        var status = await bar.SystemStatusGetAsync();

        Assert.Equal("abc", status.Device!.SerialNumber);
        Assert.Equal("1.0.2", status.Firmware!.Version);
        Assert.Equal("25.0.0", status.System!.ApiSemver);
        Assert.Equal(PowerState.Discharging, status.Power!.State);
    }

    [Fact]
    public async Task SystemStatusFirmwareGetAsync_ParsesVersionFields()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"version\":\"1.0.2\",\"target\":1,\"branch\":\"main\",\"build_date\":\"2026-01-01\",\"commit_hash\":\"abcdef1\"}";

        var firmware = await bar.SystemStatusFirmwareGetAsync();

        Assert.Equal("1.0.2", firmware.Version);
        Assert.Equal("abcdef1", firmware.CommitHash);
    }

    [Fact]
    public async Task SystemStatusSystemGetAsync_ParsesUptimeFields()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"api_semver\":\"25.0.0\",\"uptime\":\"1d\",\"boot_time\":1761582532,\"auto_update_enabled\":true}";

        var system = await bar.SystemStatusSystemGetAsync();

        Assert.Equal("1d", system.Uptime);
        Assert.True(system.AutoUpdateEnabled);
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
