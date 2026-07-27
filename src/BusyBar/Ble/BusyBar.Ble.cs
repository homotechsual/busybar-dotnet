namespace Busy.Bar;

public sealed partial class BusyBar
{
    public Task<SuccessResponse> BleEnableAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/ble/enable", options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> BleDisableAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/ble/disable", options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> BlePairingRemoveAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Delete, "busybar/ble/pairing", options: options, cancellationToken: cancellationToken);

    public Task<BleStatusResponse> BleStatusGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<BleStatusResponse>(HttpMethod.Get, "busybar/ble/status", options: options, cancellationToken: cancellationToken);
}
