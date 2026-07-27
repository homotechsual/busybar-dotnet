using System.Text.Json.Serialization;

namespace Busy.Bar;

public sealed record AccountInfo(
    [property: JsonPropertyName("linked")] bool? Linked,
    [property: JsonPropertyName("id")] string? Id,
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("user_id")] string? UserId);

public enum AccountConnectionStatus
{
    [JsonStringEnumMemberName("error")] Error,
    [JsonStringEnumMemberName("disconnected")] Disconnected,
    [JsonStringEnumMemberName("connected")] Connected
}

public sealed record AccountStatus(
    [property: JsonPropertyName("status")] AccountConnectionStatus? Status);

public enum ClientCertType
{
    [JsonStringEnumMemberName("default")] Default,
    [JsonStringEnumMemberName("custom")] Custom,
    [JsonStringEnumMemberName("none")] None
}

public sealed record AccountBackend(
    [property: JsonPropertyName("server_url")] string ServerUrl,
    [property: JsonPropertyName("client_cert_type")] ClientCertType ClientCertType,
    [property: JsonPropertyName("ignore_server_cert")] bool IgnoreServerCert);
