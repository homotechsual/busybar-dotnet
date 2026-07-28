using System.Text.Json.Serialization;

namespace Busy.Bar;

public sealed record VersionInfo(string ApiSemver);

public enum NetworkTransportType
{
    [JsonStringEnumMemberName("usb")] Usb,
    [JsonStringEnumMemberName("wifi")] Wifi
}

public sealed record NetworkInterfaceInfo(NetworkTransportType Type);

public enum FirmwareSecurity
{
    [JsonStringEnumMemberName("secure")] Secure,
    [JsonStringEnumMemberName("insecure")] Insecure,
    [JsonStringEnumMemberName("other")] Other,
    [JsonStringEnumMemberName("unknown")] Unknown
}

public sealed record StatusDevice
{
    public required string SerialNumber { get; init; }
    public required string UsbMac { get; init; }
    public string? WifiMac { get; init; }
    public string? BleMac { get; init; }
    public required bool OtpValid { get; init; }
    public string? OtpModel { get; init; }
    public long? OtpTimestamp { get; init; }
    public required FirmwareSecurity FirmwareSecurity { get; init; }
}

public sealed record StatusFirmware
{
    public required string Version { get; init; }
    public required int Target { get; init; }
    public required string Branch { get; init; }
    public required string BuildDate { get; init; }
    public required string CommitHash { get; init; }
    // Nullable (not `required`): confirmed against a real physical device (firmware 1.0.2) that "intercom_version"
    // is absent from the local device's /api/status/firmware response entirely, despite the vendored cloud-proxy
    // OpenAPI spec marking it required. See RealDeviceFixtureTests for the captured payload this guards against.
    public string? IntercomVersion { get; init; }
    public string? NwpVersion { get; init; }
    public string? MatterVersion { get; init; }
}

public sealed record StatusSystem(string ApiSemver, string Uptime, long BootTime, bool AutoUpdateEnabled);

public enum PowerState
{
    [JsonStringEnumMemberName("discharging")] Discharging,
    [JsonStringEnumMemberName("charging")] Charging,
    [JsonStringEnumMemberName("charged")] Charged
}

public sealed record StatusPower(PowerState State, int BatteryCharge, int BatteryVoltage, int BatteryCurrent, int UsbVoltage);

public sealed record SystemStatus(StatusDevice? Device, StatusFirmware? Firmware, StatusSystem? System, StatusPower? Power);

public sealed record LogDumpParams(string? Filename = null);

public sealed record LogDumpResult(string Result, string Path);
