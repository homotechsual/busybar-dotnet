using System.Text.Json.Serialization;

namespace Busy.Bar;

/// <summary>Current state of the device's Wi-Fi connection.</summary>
public enum WifiConnState
{
    /// <summary>Connection state could not be determined.</summary>
    [JsonStringEnumMemberName("unknown")] Unknown,

    /// <summary>Not connected to any Wi-Fi network.</summary>
    [JsonStringEnumMemberName("disconnected")] Disconnected,

    /// <summary>Connected to a Wi-Fi network.</summary>
    [JsonStringEnumMemberName("connected")] Connected,

    /// <summary>A connection attempt is in progress.</summary>
    [JsonStringEnumMemberName("connecting")] Connecting,

    /// <summary>Disconnecting from the current Wi-Fi network.</summary>
    [JsonStringEnumMemberName("disconnecting")] Disconnecting,

    /// <summary>Attempting to re-establish a previously active connection.</summary>
    [JsonStringEnumMemberName("reconnecting")] Reconnecting
}

/// <summary>Wi-Fi security method used by a network.</summary>
public enum WifiSecurityMethod
{
    /// <summary>No security; an open network.</summary>
    [JsonStringEnumMemberName("Open")] Open,

    /// <summary>WPA.</summary>
    [JsonStringEnumMemberName("WPA")] Wpa,

    /// <summary>WPA2.</summary>
    [JsonStringEnumMemberName("WPA2")] Wpa2,

    /// <summary>WEP.</summary>
    [JsonStringEnumMemberName("WEP")] Wep,

    /// <summary>WPA or WPA2 (exact variant not distinguished).</summary>
    [JsonStringEnumMemberName("WPA/WPA2")] WpaWpa2,

    /// <summary>WPA3.</summary>
    [JsonStringEnumMemberName("WPA3")] Wpa3,

    /// <summary>WPA2 or WPA3 (exact variant not distinguished).</summary>
    [JsonStringEnumMemberName("WPA2/WPA3")] Wpa2Wpa3,

    /// <summary>A security method not supported/recognized by the device.</summary>
    [JsonStringEnumMemberName("Unsupported")] Unsupported
}

/// <summary>How the device's IP address is assigned.</summary>
public enum WifiIpMethod
{
    /// <summary>Assigned automatically via DHCP.</summary>
    [JsonStringEnumMemberName("dhcp")] Dhcp,

    /// <summary>Manually configured with a static address.</summary>
    [JsonStringEnumMemberName("static")] Static
}

/// <summary>IP address family.</summary>
public enum WifiIpType
{
    /// <summary>IPv4.</summary>
    [JsonStringEnumMemberName("ipv4")] Ipv4,

    /// <summary>IPv6.</summary>
    [JsonStringEnumMemberName("ipv6")] Ipv6
}

/// <summary>IP configuration of the device's active Wi-Fi connection.</summary>
/// <param name="IpMethod">How the address was assigned.</param>
/// <param name="IpType">Address family of <paramref name="Address"/>.</param>
/// <param name="Address">The assigned IP address.</param>
public sealed record WifiStatusIpConfig(WifiIpMethod IpMethod, WifiIpType IpType, string Address);

/// <summary>Only <see cref="State"/> is always present; the rest are only populated when connected.</summary>
/// <param name="State">Current connection state.</param>
/// <param name="Ssid">SSID of the connected network.</param>
/// <param name="Bssid">BSSID (access point MAC address) of the connected network.</param>
/// <param name="Channel">Wi-Fi channel in use.</param>
/// <param name="Rssi">Received signal strength, in dBm.</param>
/// <param name="Security">Security method of the connected network.</param>
/// <param name="IpConfig">IP configuration of the connection.</param>
public sealed record WifiStatusResponse(
    WifiConnState State, string? Ssid, string? Bssid, int? Channel, int? Rssi,
    WifiSecurityMethod? Security, WifiStatusIpConfig? IpConfig);
