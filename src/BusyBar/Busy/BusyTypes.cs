using System.Text.Json.Serialization;

namespace Busy.Bar;

/// <summary>
/// System.Text.Json's polymorphic deserialization requires the "type" discriminator to be the first
/// property in the JSON object, or it throws <see cref="NotSupportedException"/>. Confirmed against a
/// real BUSY Bar device (see <c>RealDeviceFixtureTests</c>) that "type" is always first for both
/// <see cref="BusyTimerSettings"/> and <see cref="BusySnapshotState"/> payloads, across
/// <c>GET /busy/snapshot</c>, <c>GET /busy/profiles/{slot}</c>, and multiple discriminator values
/// (NOT_STARTED, INTERVAL) — treated as a confirmed assumption, not a theoretical risk.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(BusyTimerInfiniteSettings), "INFINITE")]
[JsonDerivedType(typeof(BusyTimerSimpleSettings), "SIMPLE")]
[JsonDerivedType(typeof(BusyTimerIntervalSettings), "INTERVAL")]
public abstract record BusyTimerSettings;

/// <summary>A timer that runs indefinitely, with no fixed duration.</summary>
public sealed record BusyTimerInfiniteSettings : BusyTimerSettings;

/// <summary>A timer that runs for a single fixed duration.</summary>
public sealed record BusyTimerSimpleSettings : BusyTimerSettings
{
    /// <summary>Total duration of the timer, in milliseconds.</summary>
    public required long TotalTimeMs { get; init; }
}

/// <summary>A timer that alternates between work and rest periods for a fixed number of cycles.</summary>
public sealed record BusyTimerIntervalSettings : BusyTimerSettings
{
    /// <summary>Duration of each work period, in milliseconds.</summary>
    public required long IntervalWorkMs { get; init; }

    /// <summary>Duration of each rest period, in milliseconds.</summary>
    public required long IntervalRestMs { get; init; }

    /// <summary>Number of work/rest cycles to run.</summary>
    public required int IntervalWorkCyclesCount { get; init; }

    /// <summary>Whether the next cycle starts automatically once the current one finishes.</summary>
    public required bool IsAutostartEnabled { get; init; }
}

/// <summary>Device-level presentation settings associated with a BUSY timer profile or snapshot.</summary>
/// <param name="Theme">Name of the visual theme to display while the timer is running.</param>
/// <param name="ShowWorkPhaseOnly">Whether to show only the work phase of an interval timer, hiding rest phases.</param>
/// <param name="TriggerSmartHome">Whether starting the timer should also trigger any paired smart home actions.</param>
public sealed record BusyBarSettings(string Theme, bool ShowWorkPhaseOnly, bool TriggerSmartHome);

/// <summary>See the discriminator-ordering note on <see cref="BusyTimerSettings"/> — the same confirmed
/// assumption applies here.</summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(BusySnapshotNotStarted), "NOT_STARTED")]
[JsonDerivedType(typeof(BusySnapshotInfinite), "INFINITE")]
[JsonDerivedType(typeof(BusySnapshotSimple), "SIMPLE")]
[JsonDerivedType(typeof(BusySnapshotInterval), "INTERVAL")]
public abstract record BusySnapshotState
{
    /// <summary>Presentation settings in effect for this snapshot, if the timer has been started.</summary>
    public BusyBarSettings? BusyBarSettings { get; init; }
}

/// <summary>No BUSY timer is currently running.</summary>
public sealed record BusySnapshotNotStarted : BusySnapshotState;

/// <summary>An infinite (no fixed duration) BUSY timer is running.</summary>
public sealed record BusySnapshotInfinite : BusySnapshotState
{
    /// <summary>Identifier of the card/profile the running timer was started from.</summary>
    public required string CardId { get; init; }

    /// <summary>Whether the timer is currently paused.</summary>
    public required bool IsPaused { get; init; }
}

/// <summary>A single fixed-duration BUSY timer is running.</summary>
public sealed record BusySnapshotSimple : BusySnapshotState
{
    /// <summary>Identifier of the card/profile the running timer was started from.</summary>
    public required string CardId { get; init; }

    /// <summary>Time remaining on the timer, in milliseconds.</summary>
    public required long TimeLeftMs { get; init; }

    /// <summary>Whether the timer is currently paused.</summary>
    public required bool IsPaused { get; init; }
}

/// <summary>A work/rest interval BUSY timer is running.</summary>
public sealed record BusySnapshotInterval : BusySnapshotState
{
    /// <summary>Identifier of the card/profile the running timer was started from.</summary>
    public required string CardId { get; init; }

    /// <summary>1-based index of the interval cycle currently in progress.</summary>
    public required int CurrentInterval { get; init; }

    /// <summary>Total duration of the current interval, in milliseconds.</summary>
    public required long CurrentIntervalTimeTotalMs { get; init; }

    /// <summary>Time remaining in the current interval, in milliseconds.</summary>
    public required long CurrentIntervalTimeLeftMs { get; init; }

    /// <summary>Whether the timer is currently paused.</summary>
    public required bool IsPaused { get; init; }

    /// <summary>The interval timer settings the running timer was started with.</summary>
    public required BusyTimerIntervalSettings IntervalSettings { get; init; }
}

/// <summary>The current state of the BUSY timer, as returned by or sent to the snapshot endpoint.</summary>
/// <param name="Snapshot">The timer state itself.</param>
/// <param name="SnapshotTimestampMs">Unix timestamp, in milliseconds, at which the snapshot was captured.</param>
public sealed record BusySnapshot(BusySnapshotState Snapshot, long SnapshotTimestampMs);

/// <summary>Identifies which of the device's two stored BUSY timer profile slots to operate on.</summary>
public enum BusyProfileSlot
{
    /// <summary>The built-in "Busy" profile slot.</summary>
    Busy,

    /// <summary>The user-configurable "Custom" profile slot.</summary>
    Custom
}

/// <summary>A stored BUSY timer profile: the timer settings and presentation settings saved under a profile slot.</summary>
public sealed record BusyProfile
{
    /// <summary>Display order of this profile relative to others.</summary>
    public required int SortOrder { get; init; }

    /// <summary>Display name of the profile.</summary>
    public required string Title { get; init; }

    /// <summary>Identifier of the profile.</summary>
    public required string Id { get; init; }

    /// <summary>The timer settings (infinite, simple, or interval) this profile starts with.</summary>
    public required BusyTimerSettings TimerSettings { get; init; }

    /// <summary>Presentation settings this profile starts with.</summary>
    public required BusyBarSettings BusyBarSettings { get; init; }

    /// <summary>Unix timestamp, in milliseconds, at which the profile was last saved.</summary>
    public required long ProfileTimestampMs { get; init; }
}
