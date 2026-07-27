namespace Busy.Bar;

public sealed partial class BusyBar
{
    public Task<SmartHomePairingInfo> SmartHomePairingGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SmartHomePairingInfo>(HttpMethod.Get, "busybar/smart_home/pairing", options: options, cancellationToken: cancellationToken);

    public Task<SmartHomePairingPayload> SmartHomePairingStartAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SmartHomePairingPayload>(HttpMethod.Post, "busybar/smart_home/pairing", options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> SmartHomePairingEraseAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Delete, "busybar/smart_home/pairing", options: options, cancellationToken: cancellationToken);

    public Task<SmartHomeSwitchState> SmartHomeSwitchGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SmartHomeSwitchState>(HttpMethod.Get, "busybar/smart_home/switch", options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> SmartHomeSwitchSetAsync(SmartHomeSwitchState state, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/smart_home/switch", jsonBody: state, options: options, cancellationToken: cancellationToken);
}
