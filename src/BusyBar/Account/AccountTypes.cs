using System.Text.Json.Serialization;

namespace Busy.Bar;

public sealed record AccountInfo(
    bool? Linked,
    string? Id,
    string? Email,
    string? UserId);

public enum AccountConnectionStatus
{
    [JsonStringEnumMemberName("error")] Error,
    [JsonStringEnumMemberName("disconnected")] Disconnected,
    [JsonStringEnumMemberName("connected")] Connected
}

public sealed record AccountStatus(
    AccountConnectionStatus? Status);

public enum ClientCertType
{
    [JsonStringEnumMemberName("default")] Default,
    [JsonStringEnumMemberName("custom")] Custom,
    [JsonStringEnumMemberName("none")] None
}

public sealed record AccountBackend(
    string ServerUrl,
    ClientCertType ClientCertType,
    bool IgnoreServerCert);
