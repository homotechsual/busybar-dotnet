using System.Text.Json;
using Busy.Bar;
using Busy.Bar.Internal;
using Xunit;

namespace BusyBar.Tests;

/// <summary>
/// Regression tests using exact JSON payloads captured from a real, physical BUSY Bar device
/// (firmware 1.0.2, api_semver 24.3.0) over USB, to prove our types deserialize real hardware
/// output correctly — not just our own synthetic test fixtures.
/// </summary>
public class RealDeviceFixtureTests
{
    [Fact]
    public void SystemStatus_DeserializesRealDevicePayload()
    {
        const string json = """
        {"device":{"serial_number":"2034305532325017002d000c","usb_mac":"0c:fa:22:00:49:f6","wifi_mac":"0c:fa:22:00:49:f7","ble_mac":"0c:fa:22:00:49:f8","otp_valid":true,"otp_model":"BB.1","otp_timestamp":1782286667,"firmware_security":"secure"},"firmware":{"version":"1.0.2","target":22,"branch":"1.0.2","build_date":"2026-07-13","commit_hash":"07e850ec","nwp_version":"1611.2.1.1.255.11.71","matter_version":"1.0"},"system":{"api_semver":"24.3.0","uptime":"02d 68h 33m 42s","boot_time":1784975939,"auto_update_enabled":true},"power":{"state":"discharging","battery_charge":99,"battery_voltage":4151,"battery_current":0,"usb_voltage":4924}}
        """;

        var status = JsonSerializer.Deserialize<SystemStatus>(json, BusyBarTransport.JsonOptions)!;

        Assert.Equal("2034305532325017002d000c", status.Device!.SerialNumber);
        Assert.Equal(FirmwareSecurity.Secure, status.Device.FirmwareSecurity);
        Assert.Equal("1.0.2", status.Firmware!.Version);
        Assert.Equal(PowerState.Discharging, status.Power!.State);
        Assert.Equal(99, status.Power.BatteryCharge);
    }

    [Fact]
    public void NameInfo_DeserializesRealDevicePayload()
    {
        var info = JsonSerializer.Deserialize<NameInfo>("""{"name":"BUSY Bar"}""", BusyBarTransport.JsonOptions)!;
        Assert.Equal("BUSY Bar", info.Name);
    }

    [Fact]
    public void DisplayBrightnessInfo_DeserializesRealDevicePayload()
    {
        var info = JsonSerializer.Deserialize<DisplayBrightnessInfo>("""{"value":"50"}""", BusyBarTransport.JsonOptions)!;
        Assert.Equal("50", info.Value);
    }

    [Fact]
    public void NetworkInterfaceInfo_DeserializesRealDevicePayload()
    {
        var info = JsonSerializer.Deserialize<NetworkInterfaceInfo>("""{"type":"usb"}""", BusyBarTransport.JsonOptions)!;
        Assert.Equal(NetworkTransportType.Usb, info.Type);
    }

    [Fact]
    public void StatusDevice_DeserializesRealDevicePayload()
    {
        const string json = """
        {"serial_number":"2034305532325017002d000c","usb_mac":"0c:fa:22:00:49:f6","wifi_mac":"0c:fa:22:00:49:f7","ble_mac":"0c:fa:22:00:49:f8","otp_valid":true,"otp_model":"BB.1","otp_timestamp":1782286667,"firmware_security":"secure"}
        """;

        var device = JsonSerializer.Deserialize<StatusDevice>(json, BusyBarTransport.JsonOptions)!;

        Assert.Equal("0c:fa:22:00:49:f6", device.UsbMac);
        Assert.Equal("BB.1", device.OtpModel);
    }

    [Fact]
    public void StatusFirmware_DeserializesRealDevicePayload()
    {
        const string json = """
        {"version":"1.0.2","target":22,"branch":"1.0.2","build_date":"2026-07-13","commit_hash":"07e850ec","nwp_version":"1611.2.1.1.255.11.71","matter_version":"1.0"}
        """;

        var firmware = JsonSerializer.Deserialize<StatusFirmware>(json, BusyBarTransport.JsonOptions)!;

        Assert.Equal(22, firmware.Target);
        Assert.Equal("07e850ec", firmware.CommitHash);
        Assert.Equal("1611.2.1.1.255.11.71", firmware.NwpVersion);
        // The real device payload omits "intercom_version" entirely (unlike the vendored cloud-proxy OpenAPI spec,
        // which marks it required) — confirming StatusFirmware.IntercomVersion must be nullable, not required.
        Assert.Null(firmware.IntercomVersion);
    }
}
