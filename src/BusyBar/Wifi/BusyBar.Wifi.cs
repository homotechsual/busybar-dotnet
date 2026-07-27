namespace Busy.Bar;

public sealed partial class BusyBar
{
    public Task<WifiStatusResponse> WifiStatusGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<WifiStatusResponse>(HttpMethod.Get, "busybar/wifi/status", options: options, cancellationToken: cancellationToken);
}
