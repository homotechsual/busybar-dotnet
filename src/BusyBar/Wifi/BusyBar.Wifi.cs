namespace Busy.Bar;

public sealed partial class BusyBar
{
    /// <summary>Gets the device's current Wi-Fi status.</summary>
    public Task<WifiStatusResponse> WifiStatusGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<WifiStatusResponse>(HttpMethod.Get, "busybar/wifi/status", options: options, cancellationToken: cancellationToken);
}
