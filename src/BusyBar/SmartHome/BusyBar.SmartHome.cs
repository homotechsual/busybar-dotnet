namespace Busy.Bar;

public sealed partial class BusyBar
{
    /// <summary>Gets the device's smart home commissioning status.</summary>
    public Task<SmartHomePairingInfo> SmartHomePairingGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SmartHomePairingInfo>(HttpMethod.Get, "busybar/smart_home/pairing", options: options, cancellationToken: cancellationToken);

    /// <summary>Starts pairing ("commissioning") the device into a Matter smart home, returning the QR/manual codes needed to complete pairing.</summary>
    public Task<SmartHomePairingPayload> SmartHomePairingStartAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SmartHomePairingPayload>(HttpMethod.Post, "busybar/smart_home/pairing", options: options, cancellationToken: cancellationToken);

    /// <summary>Erases all smart home pairing info from the device. A device restart is needed for this to take effect.</summary>
    public Task<SuccessResponse> SmartHomePairingEraseAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Delete, "busybar/smart_home/pairing", options: options, cancellationToken: cancellationToken);

    /// <summary>Gets the state of the device's emulated smart home switch.</summary>
    public Task<SmartHomeSwitchState> SmartHomeSwitchGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SmartHomeSwitchState>(HttpMethod.Get, "busybar/smart_home/switch", options: options, cancellationToken: cancellationToken);

    /// <summary>Sets the state of the device's emulated smart home switch.</summary>
    public Task<SuccessResponse> SmartHomeSwitchSetAsync(SmartHomeSwitchState state, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/smart_home/switch", jsonBody: state, options: options, cancellationToken: cancellationToken);
}
