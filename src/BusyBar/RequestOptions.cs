namespace Busy.Bar;

/// <summary>Per-call overrides, passed as the trailing options argument to every method.</summary>
public sealed class RequestOptions
{
    /// <summary>Overrides <see cref="BusyBarOptions.Timeout"/> for this call only.</summary>
    public TimeSpan? Timeout { get; init; }

    /// <summary>Cancellation token for this call. Equivalent to TS's <c>AbortSignal</c>.</summary>
    public CancellationToken CancellationToken { get; init; } = CancellationToken.None;
}
