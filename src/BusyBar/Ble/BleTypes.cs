using System.Text.Json.Serialization;

namespace Busy.Bar;

/// <summary>The device's current Bluetooth Low Energy (BLE) module state.</summary>
public enum BleStatus
{
    /// <summary>The BLE module is resetting.</summary>
    [JsonStringEnumMemberName("reset")] Reset,

    /// <summary>The BLE module is initializing.</summary>
    [JsonStringEnumMemberName("initialization")] Initialization,

    /// <summary>The BLE module is disabled.</summary>
    [JsonStringEnumMemberName("disabled")] Disabled,

    /// <summary>The BLE module is enabled but not yet advertising as connectable.</summary>
    [JsonStringEnumMemberName("enabled")] Enabled,

    /// <summary>The BLE module is advertising and available for a remote device to connect to.</summary>
    [JsonStringEnumMemberName("connectable")] Connectable,

    /// <summary>The BLE module is connected to a remote device.</summary>
    [JsonStringEnumMemberName("connected")] Connected,

    /// <summary>The BLE module encountered an internal error.</summary>
    [JsonStringEnumMemberName("internal error")] InternalError
}

/// <summary>The device's current BLE status.</summary>
/// <param name="Status">The BLE module's current state.</param>
/// <param name="Address">The remote device's address. Only present when <paramref name="Status"/> is <see cref="BleStatus.Connected"/>.</param>
public sealed record BleStatusResponse(BleStatus Status, string? Address);
