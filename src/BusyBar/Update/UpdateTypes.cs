using System.Text.Json.Serialization;

namespace Busy.Bar;

public enum UpdateEvent
{
    [JsonStringEnumMemberName("session_start")] SessionStart,
    [JsonStringEnumMemberName("session_stop")] SessionStop,
    [JsonStringEnumMemberName("action_begin")] ActionBegin,
    [JsonStringEnumMemberName("action_done")] ActionDone,
    [JsonStringEnumMemberName("detail_change")] DetailChange,
    [JsonStringEnumMemberName("action_progress")] ActionProgress,
    [JsonStringEnumMemberName("none")] None
}

public enum UpdateAction
{
    [JsonStringEnumMemberName("download")] Download,
    [JsonStringEnumMemberName("sha_verification")] ShaVerification,
    [JsonStringEnumMemberName("unpack")] Unpack,
    [JsonStringEnumMemberName("prepare")] Prepare,
    [JsonStringEnumMemberName("apply")] Apply,
    [JsonStringEnumMemberName("none")] None
}

public enum UpdateInstallResultStatus
{
    [JsonStringEnumMemberName("ok")] Ok,
    [JsonStringEnumMemberName("battery_low")] BatteryLow,
    [JsonStringEnumMemberName("busy")] Busy,
    [JsonStringEnumMemberName("download_failure")] DownloadFailure,
    [JsonStringEnumMemberName("download_abort")] DownloadAbort,
    [JsonStringEnumMemberName("sha_mismatch")] ShaMismatch,
    [JsonStringEnumMemberName("unpack_staging_dir_failure")] UnpackStagingDirFailure,
    [JsonStringEnumMemberName("unpack_archive_open_failure")] UnpackArchiveOpenFailure,
    [JsonStringEnumMemberName("unpack_archive_unpack_failure")] UnpackArchiveUnpackFailure,
    [JsonStringEnumMemberName("install_manifest_not_found")] InstallManifestNotFound,
    [JsonStringEnumMemberName("install_manifest_invalid")] InstallManifestInvalid,
    [JsonStringEnumMemberName("install_session_config_failure")] InstallSessionConfigFailure,
    [JsonStringEnumMemberName("install_pointer_setup_failure")] InstallPointerSetupFailure,
    [JsonStringEnumMemberName("unknown_failure")] UnknownFailure
}

public enum UpdateCheckEvent
{
    [JsonStringEnumMemberName("start")] Start,
    [JsonStringEnumMemberName("stop")] Stop,
    [JsonStringEnumMemberName("none")] None
}

public enum UpdateCheckResultStatus
{
    [JsonStringEnumMemberName("available")] Available,
    [JsonStringEnumMemberName("not_available")] NotAvailable,
    [JsonStringEnumMemberName("failure")] Failure,
    [JsonStringEnumMemberName("none")] None
}

public sealed record UpdateDownloadProgress(long? SpeedBytesPerSec, long? ReceivedBytes, long? TotalBytes);

public sealed record UpdateInstallStatus(
    bool? IsAllowed, UpdateEvent? Event, UpdateAction? Action,
    UpdateInstallResultStatus? Status, string? Detail, UpdateDownloadProgress? Download);

public sealed record UpdateCheckStatus(string? AvailableVersion, UpdateCheckEvent? Event, UpdateCheckResultStatus? Status);

public sealed record UpdateStatus(UpdateInstallStatus? Install, UpdateCheckStatus? Check);

public sealed record UpdateChangelogParams(string Version);

public sealed record UpdateChangelogResult(string? Changelog);

public sealed record UpdateInstallParams(string Version);

/// <summary>All fields are optional for POST requests; only provided fields are updated.</summary>
public sealed record AutoupdateSettings(bool? IsEnabled = null, string? IntervalStart = null, string? IntervalEnd = null);
