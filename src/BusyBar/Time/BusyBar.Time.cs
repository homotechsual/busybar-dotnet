namespace Busy.Bar;

public sealed partial class BusyBar
{
    public Task<TimestampInfo> TimeGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<TimestampInfo>(HttpMethod.Get, "busybar/time", options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> TimeSetTimestampAsync(TimeSetTimestampParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/time/timestamp",
            query: new Dictionary<string, string?> { ["timestamp"] = parameters.Timestamp }, options: options, cancellationToken: cancellationToken);

    public Task<TimezoneInfo> TimeTimezoneGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<TimezoneInfo>(HttpMethod.Get, "busybar/time/timezone", options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> TimeTimezoneSetAsync(TimeSetTimezoneParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/time/timezone",
            query: new Dictionary<string, string?> { ["timezone"] = parameters.Timezone }, options: options, cancellationToken: cancellationToken);

    public Task<TimezoneListResponse> TimeTzlistGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<TimezoneListResponse>(HttpMethod.Get, "busybar/time/tzlist", options: options, cancellationToken: cancellationToken);
}
