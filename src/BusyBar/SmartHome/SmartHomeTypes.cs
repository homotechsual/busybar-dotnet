using System.Text.Json.Serialization;

namespace Busy.Bar;

/// <summary>Latest state of the smart home pairing (Matter "commissioning") process.</summary>
public enum PairingStatusValue
{
    /// <summary>No pairing attempt has occurred during the current power cycle. Not recorded across reboots.</summary>
    [JsonStringEnumMemberName("never_started")] NeverStarted,

    /// <summary>A pairing attempt is currently in progress.</summary>
    [JsonStringEnumMemberName("started")] Started,

    /// <summary>The most recent pairing attempt completed successfully.</summary>
    [JsonStringEnumMemberName("completed_successfully")] CompletedSuccessfully,

    /// <summary>The most recent pairing attempt failed.</summary>
    [JsonStringEnumMemberName("failed")] Failed
}

/// <summary>Latest recorded state of the smart home pairing process.</summary>
/// <param name="Value">Latest pairing status.</param>
/// <param name="Timestamp">UTC Unix second timestamp of the latest status update. Only present when a status update has occurred.</param>
public sealed record SmartHomeLatestPairingStatus(PairingStatusValue? Value, long? Timestamp);

/// <summary>Current smart home commissioning status of the device.</summary>
/// <param name="FabricCount">Number of smart homes (Matter "fabrics") that this device is paired with ("commissioned into").</param>
/// <param name="LatestPairingStatus">Latest recorded state of the pairing process.</param>
public sealed record SmartHomePairingInfo(int? FabricCount, SmartHomeLatestPairingStatus? LatestPairingStatus);

/// <summary>Set of information for pairing with a Matter smart home.</summary>
/// <param name="AvailableUntil">Pairing with the provided payload is possible before this UTC Unix millisecond timestamp, encoded as a numeric string.</param>
/// <param name="QrCode">Payload of the QR code for pairing with the smart home.</param>
/// <param name="ManualCode">Manual code for pairing with the smart home.</param>
public sealed record SmartHomePairingPayload(string? AvailableUntil, string? QrCode, string? ManualCode);

/// <summary>State of the emulated smart home switch on device startup.</summary>
public enum SwitchStartup
{
    /// <summary>The switch starts off.</summary>
    [JsonStringEnumMemberName("off")] Off,

    /// <summary>The switch starts on.</summary>
    [JsonStringEnumMemberName("on")] On,

    /// <summary>The switch starts in the opposite state to the one it was last set to.</summary>
    [JsonStringEnumMemberName("toggle")] Toggle,

    /// <summary>The switch starts in whatever state it was last set to.</summary>
    [JsonStringEnumMemberName("last")] Last
}

/// <summary>State of the device's emulated smart home switch.</summary>
/// <param name="State">Current state of the emulated switch.</param>
/// <param name="Startup">State of the emulated switch on startup. Never sent by the server, but can be specified by the client.</param>
public sealed record SmartHomeSwitchState(bool? State, SwitchStartup? Startup = null);
