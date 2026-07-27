using System.Text.Json.Serialization;

namespace Busy.Bar;

public enum PairingStatusValue
{
    [JsonStringEnumMemberName("never_started")] NeverStarted,
    [JsonStringEnumMemberName("started")] Started,
    [JsonStringEnumMemberName("completed_successfully")] CompletedSuccessfully,
    [JsonStringEnumMemberName("failed")] Failed
}

public sealed record SmartHomeLatestPairingStatus(PairingStatusValue? Value, long? Timestamp);

public sealed record SmartHomePairingInfo(int? FabricCount, SmartHomeLatestPairingStatus? LatestPairingStatus);

public sealed record SmartHomePairingPayload(string? AvailableUntil, string? QrCode, string? ManualCode);

public enum SwitchStartup
{
    [JsonStringEnumMemberName("off")] Off,
    [JsonStringEnumMemberName("on")] On,
    [JsonStringEnumMemberName("toggle")] Toggle,
    [JsonStringEnumMemberName("last")] Last
}

public sealed record SmartHomeSwitchState(bool? State, SwitchStartup? Startup = null);
