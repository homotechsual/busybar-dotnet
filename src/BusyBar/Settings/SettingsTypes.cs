using System.Text.Json.Serialization;

namespace Busy.Bar;

public enum HttpAccessMode
{
    [JsonStringEnumMemberName("disabled")] Disabled,
    [JsonStringEnumMemberName("enabled")] Enabled,
    [JsonStringEnumMemberName("key")] Key
}

public sealed record HttpAccessInfo(HttpAccessMode? Mode, bool? KeyValid);

/// <summary><paramref name="Key"/> is required when <paramref name="Mode"/> is <see cref="HttpAccessMode.Key"/> (4-10 digits).</summary>
public sealed record AccessSetParams(HttpAccessMode Mode, string? Key = null);

public sealed record NameInfo(string Name);
