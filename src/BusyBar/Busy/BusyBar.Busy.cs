namespace Busy.Bar;

public sealed partial class BusyBar
{
    /// <summary>Retrieves the current state of the BUSY timer, as a snapshot.</summary>
    public Task<BusySnapshot> BusySnapshotGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<BusySnapshot>(HttpMethod.Get, "busybar/busy/snapshot", options: options, cancellationToken: cancellationToken);

    /// <summary>Starts (or resumes) the BUSY timer from the given snapshot.</summary>
    public Task<SuccessResponse> BusySnapshotSetAsync(BusySnapshot snapshot, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Put, "busybar/busy/snapshot", jsonBody: snapshot, options: options, cancellationToken: cancellationToken);

    /// <summary>Retrieves the BUSY timer profile stored under the given slot.</summary>
    public Task<BusyProfile> BusyProfileGetAsync(BusyProfileSlot slot, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<BusyProfile>(HttpMethod.Get, $"busybar/busy/profiles/{ToApiString(slot)}", options: options, cancellationToken: cancellationToken);

    /// <summary>Saves a BUSY timer profile under the given slot.</summary>
    public Task<SuccessResponse> BusyProfileSetAsync(BusyProfileSlot slot, BusyProfile profile, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Put, $"busybar/busy/profiles/{ToApiString(slot)}", jsonBody: profile, options: options, cancellationToken: cancellationToken);

    private static string ToApiString(BusyProfileSlot slot) => slot switch
    {
        BusyProfileSlot.Busy => "busy",
        BusyProfileSlot.Custom => "custom",
        _ => throw new ArgumentOutOfRangeException(nameof(slot))
    };
}
