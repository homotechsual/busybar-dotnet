namespace Busy.Bar;

public sealed partial class BusyBar
{
    /// <summary>Retrieves the BUSY Cloud account linked to this device, if any.</summary>
    public Task<AccountInfo> AccountInfoGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<AccountInfo>(HttpMethod.Get, "busybar/account/info", options: options, cancellationToken: cancellationToken);

    /// <summary>Retrieves the device's current MQTT connection status to BUSY Cloud.</summary>
    public Task<AccountStatus> AccountStatusGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<AccountStatus>(HttpMethod.Get, "busybar/account/status", options: options, cancellationToken: cancellationToken);

    /// <summary>Retrieves the device's MQTT backend configuration.</summary>
    public Task<AccountBackend> AccountBackendGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<AccountBackend>(HttpMethod.Get, "busybar/account/backend", options: options, cancellationToken: cancellationToken);
}
