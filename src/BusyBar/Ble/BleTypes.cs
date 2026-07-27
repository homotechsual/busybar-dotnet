using System.Text.Json.Serialization;

namespace Busy.Bar;

public enum BleStatus
{
    [JsonStringEnumMemberName("reset")] Reset,
    [JsonStringEnumMemberName("initialization")] Initialization,
    [JsonStringEnumMemberName("disabled")] Disabled,
    [JsonStringEnumMemberName("enabled")] Enabled,
    [JsonStringEnumMemberName("connectable")] Connectable,
    [JsonStringEnumMemberName("connected")] Connected,
    [JsonStringEnumMemberName("internal error")] InternalError
}

public sealed record BleStatusResponse(BleStatus Status, string? Address);
