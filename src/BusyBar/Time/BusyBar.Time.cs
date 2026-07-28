namespace Busy.Bar;

public sealed partial class BusyBar
{
    /// <summary>Gets the device's current real-time-clock timestamp, with timezone, in ISO 8601 format.</summary>
    public Task<TimestampInfo> TimeGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<TimestampInfo>(HttpMethod.Get, "busybar/time", options: options, cancellationToken: cancellationToken);

    /// <summary>Sets the device's real-time-clock timestamp.</summary>
    public Task<SuccessResponse> TimeSetTimestampAsync(TimeSetTimestampParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/time/timestamp",
            query: new Dictionary<string, string?> { ["timestamp"] = parameters.Timestamp }, options: options, cancellationToken: cancellationToken);

    /// <summary>Gets the device's current timezone.</summary>
    public Task<TimezoneInfo> TimeTimezoneGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<TimezoneInfo>(HttpMethod.Get, "busybar/time/timezone", options: options, cancellationToken: cancellationToken);

    /// <summary>Sets the device's timezone. Use <see cref="TimeTzlistGetAsync"/> to get the list of accepted names.</summary>
    public Task<SuccessResponse> TimeTimezoneSetAsync(TimeSetTimezoneParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/time/timezone",
            query: new Dictionary<string, string?> { ["timezone"] = parameters.Timezone }, options: options, cancellationToken: cancellationToken);

    /// <summary>Gets the list of time zones accepted by <see cref="TimeTimezoneSetAsync"/>.</summary>
    public Task<TimezoneListResponse> TimeTzlistGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<TimezoneListResponse>(HttpMethod.Get, "busybar/time/tzlist", options: options, cancellationToken: cancellationToken);
}
