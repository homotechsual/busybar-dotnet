namespace Busy.Bar;

public sealed partial class BusyBar
{
    /// <summary>Enables the BLE module and starts advertising.</summary>
    public Task<SuccessResponse> BleEnableAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/ble/enable", options: options, cancellationToken: cancellationToken);

    /// <summary>Disables the BLE module, stopping advertising.</summary>
    public Task<SuccessResponse> BleDisableAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/ble/disable", options: options, cancellationToken: cancellationToken);

    /// <summary>Removes the pairing with the previously paired device, making the device discoverable again.</summary>
    public Task<SuccessResponse> BlePairingRemoveAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Delete, "busybar/ble/pairing", options: options, cancellationToken: cancellationToken);

    /// <summary>Retrieves the device's current BLE status.</summary>
    public Task<BleStatusResponse> BleStatusGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<BleStatusResponse>(HttpMethod.Get, "busybar/ble/status", options: options, cancellationToken: cancellationToken);
}
