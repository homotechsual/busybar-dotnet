using System.Text.Json.Serialization;

namespace Busy.Bar;

public enum WifiConnState
{
    [JsonStringEnumMemberName("unknown")] Unknown,
    [JsonStringEnumMemberName("disconnected")] Disconnected,
    [JsonStringEnumMemberName("connected")] Connected,
    [JsonStringEnumMemberName("connecting")] Connecting,
    [JsonStringEnumMemberName("disconnecting")] Disconnecting,
    [JsonStringEnumMemberName("reconnecting")] Reconnecting
}

public enum WifiSecurityMethod
{
    [JsonStringEnumMemberName("Open")] Open,
    [JsonStringEnumMemberName("WPA")] Wpa,
    [JsonStringEnumMemberName("WPA2")] Wpa2,
    [JsonStringEnumMemberName("WEP")] Wep,
    [JsonStringEnumMemberName("WPA/WPA2")] WpaWpa2,
    [JsonStringEnumMemberName("WPA3")] Wpa3,
    [JsonStringEnumMemberName("WPA2/WPA3")] Wpa2Wpa3,
    [JsonStringEnumMemberName("Unsupported")] Unsupported
}

public enum WifiIpMethod
{
    [JsonStringEnumMemberName("dhcp")] Dhcp,
    [JsonStringEnumMemberName("static")] Static
}

public enum WifiIpType
{
    [JsonStringEnumMemberName("ipv4")] Ipv4,
    [JsonStringEnumMemberName("ipv6")] Ipv6
}

public sealed record WifiStatusIpConfig(WifiIpMethod IpMethod, WifiIpType IpType, string Address);

/// <summary>Only <see cref="State"/> is always present; the rest are only populated when connected.</summary>
public sealed record WifiStatusResponse(
    WifiConnState State, string? Ssid, string? Bssid, int? Channel, int? Rssi,
    WifiSecurityMethod? Security, WifiStatusIpConfig? IpConfig);
