using System.Text.Json.Serialization;

namespace Busy.Bar;

/// <summary>
/// System.Text.Json's polymorphic deserialization requires the "type" discriminator to be the first
/// property in the JSON object, or it throws <see cref="NotSupportedException"/>. Confirmed against a
/// real BUSY Bar device (see <c>RealDeviceFixtureTests</c>) that "type" is always first for both
/// <see cref="BusyTimerSettings"/> and <see cref="BusySnapshotState"/> payloads, across
/// GET /busy/snapshot, GET /busy/profiles/{slot}, and multiple discriminator values
/// (NOT_STARTED, INTERVAL) — treated as a confirmed assumption, not a theoretical risk.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(BusyTimerInfiniteSettings), "INFINITE")]
[JsonDerivedType(typeof(BusyTimerSimpleSettings), "SIMPLE")]
[JsonDerivedType(typeof(BusyTimerIntervalSettings), "INTERVAL")]
public abstract record BusyTimerSettings;

public sealed record BusyTimerInfiniteSettings : BusyTimerSettings;

public sealed record BusyTimerSimpleSettings : BusyTimerSettings
{
    public required long TotalTimeMs { get; init; }
}

public sealed record BusyTimerIntervalSettings : BusyTimerSettings
{
    public required long IntervalWorkMs { get; init; }
    public required long IntervalRestMs { get; init; }
    public required int IntervalWorkCyclesCount { get; init; }
    public required bool IsAutostartEnabled { get; init; }
}

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
    public BusyBarSettings? BusyBarSettings { get; init; }
}

public sealed record BusySnapshotNotStarted : BusySnapshotState;

public sealed record BusySnapshotInfinite : BusySnapshotState
{
    public required string CardId { get; init; }
    public required bool IsPaused { get; init; }
}

public sealed record BusySnapshotSimple : BusySnapshotState
{
    public required string CardId { get; init; }
    public required long TimeLeftMs { get; init; }
    public required bool IsPaused { get; init; }
}

public sealed record BusySnapshotInterval : BusySnapshotState
{
    public required string CardId { get; init; }
    public required int CurrentInterval { get; init; }
    public required long CurrentIntervalTimeTotalMs { get; init; }
    public required long CurrentIntervalTimeLeftMs { get; init; }
    public required bool IsPaused { get; init; }
    public required BusyTimerIntervalSettings IntervalSettings { get; init; }
}

public sealed record BusySnapshot(BusySnapshotState Snapshot, long SnapshotTimestampMs);

public enum BusyProfileSlot
{
    Busy,
    Custom
}

public sealed record BusyProfile
{
    public required int SortOrder { get; init; }
    public required string Title { get; init; }
    public required string Id { get; init; }
    public required BusyTimerSettings TimerSettings { get; init; }
    public required BusyBarSettings BusyBarSettings { get; init; }
    public required long ProfileTimestampMs { get; init; }
}
