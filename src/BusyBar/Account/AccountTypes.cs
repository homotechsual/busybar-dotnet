using System.Text.Json.Serialization;

namespace Busy.Bar;

/// <summary>The BUSY Cloud account linked to this device, if any.</summary>
/// <param name="Linked">Whether a BUSY Cloud account is currently linked to this device.</param>
/// <param name="Id">Identifier of the account link, if linked.</param>
/// <param name="Email">Email address of the linked account, if linked.</param>
/// <param name="UserId">Identifier of the linked BUSY Cloud user, if linked.</param>
public sealed record AccountInfo(
    bool? Linked,
    string? Id,
    string? Email,
    string? UserId);

/// <summary>The device's MQTT connection status to BUSY Cloud.</summary>
public enum AccountConnectionStatus
{
    /// <summary>The MQTT connection is in an error state.</summary>
    [JsonStringEnumMemberName("error")] Error,

    /// <summary>The device is not currently connected to BUSY Cloud over MQTT.</summary>
    [JsonStringEnumMemberName("disconnected")] Disconnected,

    /// <summary>The device is connected to BUSY Cloud over MQTT.</summary>
    [JsonStringEnumMemberName("connected")] Connected
}

/// <summary>The device's current MQTT connection status.</summary>
/// <param name="Status">The MQTT connection status.</param>
public sealed record AccountStatus(
    AccountConnectionStatus? Status);

/// <summary>Which client certificate the device uses when connecting to the MQTT backend.</summary>
public enum ClientCertType
{
    /// <summary>Uses the device's built-in default client certificate.</summary>
    [JsonStringEnumMemberName("default")] Default,

    /// <summary>Uses a custom client certificate configured on the device.</summary>
    [JsonStringEnumMemberName("custom")] Custom,

    /// <summary>No client certificate is used.</summary>
    [JsonStringEnumMemberName("none")] None
}

/// <summary>MQTT backend configuration the device uses to connect to BUSY Cloud.</summary>
/// <param name="ServerUrl">MQTT server URL to connect to.</param>
/// <param name="ClientCertType">Client certificate type to use.</param>
/// <param name="IgnoreServerCert">Whether to ignore (skip validating) the MQTT server's certificate.</param>
public sealed record AccountBackend(
    string ServerUrl,
    ClientCertType ClientCertType,
    bool IgnoreServerCert);
