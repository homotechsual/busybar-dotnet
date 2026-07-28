using System.Text.Json.Serialization;

namespace Busy.Bar;

/// <summary>Whether, and how, the device's local HTTP API is reachable over Wi-Fi.</summary>
public enum HttpAccessMode
{
    /// <summary>The local HTTP API is not reachable over Wi-Fi.</summary>
    [JsonStringEnumMemberName("disabled")] Disabled,

    /// <summary>The local HTTP API is reachable over Wi-Fi without authentication.</summary>
    [JsonStringEnumMemberName("enabled")] Enabled,

    /// <summary>The local HTTP API is reachable over Wi-Fi and requires the configured access key.</summary>
    [JsonStringEnumMemberName("key")] Key
}

/// <summary>Current HTTP API access over Wi-Fi configuration.</summary>
/// <param name="Mode">Current access mode.</param>
/// <param name="KeyValid">Whether an access key has been set and is valid.</param>
public sealed record HttpAccessInfo(HttpAccessMode? Mode, bool? KeyValid);

/// <summary><paramref name="Key"/> is required when <paramref name="Mode"/> is <see cref="HttpAccessMode.Key"/> (4-10 digits).</summary>
public sealed record AccessSetParams(HttpAccessMode Mode, string? Key = null);

/// <summary>The device's display name.</summary>
/// <param name="Name">Device name (letters, digits, spaces, and common punctuation; 1-20 characters).</param>
public sealed record NameInfo(string Name);
