using System.Text.Json.Serialization;

namespace Busy.Bar;

/// <summary>The device's local HTTP API version.</summary>
/// <param name="ApiSemver">API version, in SemVer format.</param>
public sealed record VersionInfo(string ApiSemver);

/// <summary>The transport a device's network connection uses.</summary>
public enum NetworkTransportType
{
    /// <summary>Connected over USB ethernet.</summary>
    [JsonStringEnumMemberName("usb")] Usb,

    /// <summary>Connected over Wi-Fi.</summary>
    [JsonStringEnumMemberName("wifi")] Wifi
}

/// <summary>The device's current network connection type.</summary>
/// <param name="Type">Connection type currently in use.</param>
public sealed record NetworkInterfaceInfo(NetworkTransportType Type);

/// <summary>
/// Summary of firmware signature protection, derived from the wireless coprocessor (Si917) NWP and M4
/// signature verification state.
/// </summary>
public enum FirmwareSecurity
{
    /// <summary>Both NWP and M4 firmware signature verification are active.</summary>
    [JsonStringEnumMemberName("secure")] Secure,

    /// <summary>Neither NWP nor M4 firmware signature verification is active.</summary>
    [JsonStringEnumMemberName("insecure")] Insecure,

    /// <summary>A mixed state: exactly one of NWP or M4 firmware signature verification is active.</summary>
    [JsonStringEnumMemberName("other")] Other,

    /// <summary>Coprocessor info is not ready yet, so the security state cannot be determined.</summary>
    [JsonStringEnumMemberName("unknown")] Unknown
}

/// <summary>Hardware identity and provisioning information for the device.</summary>
public sealed record StatusDevice
{
    /// <summary>Device serial number.</summary>
    public required string SerialNumber { get; init; }

    /// <summary>MAC address of the USB ethernet interface.</summary>
    public required string UsbMac { get; init; }

    /// <summary>MAC address of the Wi-Fi interface.</summary>
    public string? WifiMac { get; init; }

    /// <summary>MAC address of the BLE interface.</summary>
    public string? BleMac { get; init; }

    /// <summary>Whether the device's one-time-programmable provisioning data is valid.</summary>
    public required bool OtpValid { get; init; }

    /// <summary>Device model code.</summary>
    public string? OtpModel { get; init; }

    /// <summary>Production Unix timestamp.</summary>
    public long? OtpTimestamp { get; init; }

    /// <summary>Summary of firmware signature protection currently in effect.</summary>
    public required FirmwareSecurity FirmwareSecurity { get; init; }
}

/// <summary>Firmware build and version information for the device.</summary>
public sealed record StatusFirmware
{
    /// <summary>Firmware version.</summary>
    public required string Version { get; init; }

    /// <summary>Firmware target code.</summary>
    public required int Target { get; init; }

    /// <summary>Git branch name the firmware was built from.</summary>
    public required string Branch { get; init; }

    /// <summary>Firmware build date.</summary>
    public required string BuildDate { get; init; }

    /// <summary>Git commit hash the firmware was built from (may include a "-dirty" suffix).</summary>
    public required string CommitHash { get; init; }

    /// <summary>Intercom handshake version string (a forced version, a git hash, or "intercom" if the check is disabled).</summary>
    // Nullable (not `required`): confirmed against a real physical device (firmware 1.0.2) that "intercom_version"
    // is absent from the local device's /api/status/firmware response entirely, despite the vendored cloud-proxy
    // OpenAPI spec marking it required. See RealDeviceFixtureTests for the captured payload this guards against.
    public string? IntercomVersion { get; init; }

    /// <summary>Radio (wireless coprocessor) firmware version.</summary>
    public string? NwpVersion { get; init; }

    /// <summary>Matter protocol version supported by the device.</summary>
    public string? MatterVersion { get; init; }
}

/// <summary>General system status information.</summary>
/// <param name="ApiSemver">API version, in SemVer format.</param>
/// <param name="Uptime">Formatted system uptime.</param>
/// <param name="BootTime">Unix timestamp at which the system last booted.</param>
/// <param name="AutoUpdateEnabled">Whether automatic firmware updates are enabled.</param>
public sealed record StatusSystem(string ApiSemver, string Uptime, long BootTime, bool AutoUpdateEnabled);

/// <summary>Current power/charging state of the device.</summary>
public enum PowerState
{
    /// <summary>Running on battery power; the battery is discharging.</summary>
    [JsonStringEnumMemberName("discharging")] Discharging,

    /// <summary>Connected to external power and the battery is charging.</summary>
    [JsonStringEnumMemberName("charging")] Charging,

    /// <summary>Connected to external power and the battery is fully charged.</summary>
    [JsonStringEnumMemberName("charged")] Charged
}

/// <summary>Power and battery status.</summary>
/// <param name="State">Current power/charging state.</param>
/// <param name="BatteryCharge">Battery charge, as a percentage.</param>
/// <param name="BatteryVoltage">Battery voltage, in millivolts.</param>
/// <param name="BatteryCurrent">Battery current, in milliamps. Negative values indicate discharge.</param>
/// <param name="UsbVoltage">USB input voltage, in millivolts.</param>
public sealed record StatusPower(PowerState State, int BatteryCharge, int BatteryVoltage, int BatteryCurrent, int UsbVoltage);

/// <summary>Combined device, firmware, system, and power status.</summary>
/// <param name="Device">Hardware identity and provisioning information.</param>
/// <param name="Firmware">Firmware build and version information.</param>
/// <param name="System">General system status information.</param>
/// <param name="Power">Power and battery status.</param>
public sealed record SystemStatus(StatusDevice? Device, StatusFirmware? Firmware, StatusSystem? System, StatusPower? Power);

/// <summary>Destination for a log dump.</summary>
/// <param name="Filename">Destination file name, without extension. Defaults to <c>log</c>, written under <c>/ext</c>.</param>
public sealed record LogDumpParams(string? Filename = null);

/// <summary>Result of dumping the in-memory log buffer to a file.</summary>
/// <param name="Result">Result status string.</param>
/// <param name="Path">Full path to the written log file.</param>
public sealed record LogDumpResult(string Result, string Path);
