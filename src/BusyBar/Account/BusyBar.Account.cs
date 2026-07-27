namespace Busy.Bar;

public sealed partial class BusyBar
{
    public Task<AccountInfo> AccountInfoGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<AccountInfo>(HttpMethod.Get, "busybar/account/info", options: options, cancellationToken: cancellationToken);

    public Task<AccountStatus> AccountStatusGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<AccountStatus>(HttpMethod.Get, "busybar/account/status", options: options, cancellationToken: cancellationToken);

    public Task<AccountBackend> AccountBackendGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<AccountBackend>(HttpMethod.Get, "busybar/account/backend", options: options, cancellationToken: cancellationToken);
}
