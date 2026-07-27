using System.Text.Json.Serialization;

namespace Busy.Bar;

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
