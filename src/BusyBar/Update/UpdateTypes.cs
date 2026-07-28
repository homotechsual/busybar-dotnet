using System.Text.Json.Serialization;

namespace Busy.Bar;

/// <summary>Lifecycle event most recently reported by the firmware update/installation process.</summary>
public enum UpdateEvent
{
    /// <summary>An update installation session has started.</summary>
    [JsonStringEnumMemberName("session_start")] SessionStart,

    /// <summary>The update installation session has ended.</summary>
    [JsonStringEnumMemberName("session_stop")] SessionStop,

    /// <summary>A new action within the update process (see <see cref="UpdateAction"/>) has begun.</summary>
    [JsonStringEnumMemberName("action_begin")] ActionBegin,

    /// <summary>The current action within the update process has completed.</summary>
    [JsonStringEnumMemberName("action_done")] ActionDone,

    /// <summary>The <see cref="UpdateInstallStatus.Detail"/> string for the current action has changed.</summary>
    [JsonStringEnumMemberName("detail_change")] DetailChange,

    /// <summary>Progress was made on the current action (e.g. more bytes downloaded).</summary>
    [JsonStringEnumMemberName("action_progress")] ActionProgress,

    /// <summary>No update event is currently active.</summary>
    [JsonStringEnumMemberName("none")] None
}

/// <summary>Step of the firmware update/installation process currently in progress.</summary>
public enum UpdateAction
{
    /// <summary>Downloading the update package.</summary>
    [JsonStringEnumMemberName("download")] Download,

    /// <summary>Verifying the SHA checksum of the downloaded update package.</summary>
    [JsonStringEnumMemberName("sha_verification")] ShaVerification,

    /// <summary>Unpacking the update package.</summary>
    [JsonStringEnumMemberName("unpack")] Unpack,

    /// <summary>Preparing the unpacked update contents for installation.</summary>
    [JsonStringEnumMemberName("prepare")] Prepare,

    /// <summary>Applying the update, typically followed by a device reboot.</summary>
    [JsonStringEnumMemberName("apply")] Apply,

    /// <summary>No update action is currently in progress.</summary>
    [JsonStringEnumMemberName("none")] None
}

/// <summary>Current or last result status of the firmware update/installation process.</summary>
public enum UpdateInstallResultStatus
{
    /// <summary>The current or last operation completed successfully.</summary>
    [JsonStringEnumMemberName("ok")] Ok,

    /// <summary>Installation was blocked because the battery charge is too low.</summary>
    [JsonStringEnumMemberName("battery_low")] BatteryLow,

    /// <summary>Another update operation is already in progress.</summary>
    [JsonStringEnumMemberName("busy")] Busy,

    /// <summary>The update package failed to download.</summary>
    [JsonStringEnumMemberName("download_failure")] DownloadFailure,

    /// <summary>The download was aborted, e.g. via <see cref="BusyBar.UpdateAbortDownloadAsync"/>.</summary>
    [JsonStringEnumMemberName("download_abort")] DownloadAbort,

    /// <summary>The downloaded package's SHA checksum did not match the expected value.</summary>
    [JsonStringEnumMemberName("sha_mismatch")] ShaMismatch,

    /// <summary>Failed to create a temporary staging directory for the update package.</summary>
    [JsonStringEnumMemberName("unpack_staging_dir_failure")] UnpackStagingDirFailure,

    /// <summary>Failed to open the downloaded update archive.</summary>
    [JsonStringEnumMemberName("unpack_archive_open_failure")] UnpackArchiveOpenFailure,

    /// <summary>Failed to unpack the contents of the update archive.</summary>
    [JsonStringEnumMemberName("unpack_archive_unpack_failure")] UnpackArchiveUnpackFailure,

    /// <summary>The update package's install manifest could not be found.</summary>
    [JsonStringEnumMemberName("install_manifest_not_found")] InstallManifestNotFound,

    /// <summary>The update package's install manifest was invalid.</summary>
    [JsonStringEnumMemberName("install_manifest_invalid")] InstallManifestInvalid,

    /// <summary>Failed to configure the installation session.</summary>
    [JsonStringEnumMemberName("install_session_config_failure")] InstallSessionConfigFailure,

    /// <summary>Failed to set up the boot pointer for the newly installed firmware.</summary>
    [JsonStringEnumMemberName("install_pointer_setup_failure")] InstallPointerSetupFailure,

    /// <summary>An unspecified failure occurred.</summary>
    [JsonStringEnumMemberName("unknown_failure")] UnknownFailure
}

/// <summary>Lifecycle event most recently reported by the firmware update availability check.</summary>
public enum UpdateCheckEvent
{
    /// <summary>An update availability check has started.</summary>
    [JsonStringEnumMemberName("start")] Start,

    /// <summary>The update availability check has finished.</summary>
    [JsonStringEnumMemberName("stop")] Stop,

    /// <summary>No update availability check is currently active.</summary>
    [JsonStringEnumMemberName("none")] None
}

/// <summary>Result of the firmware update availability check.</summary>
public enum UpdateCheckResultStatus
{
    /// <summary>An update is available.</summary>
    [JsonStringEnumMemberName("available")] Available,

    /// <summary>No update is available; the device is already on the latest version.</summary>
    [JsonStringEnumMemberName("not_available")] NotAvailable,

    /// <summary>The update availability check failed.</summary>
    [JsonStringEnumMemberName("failure")] Failure,

    /// <summary>No check result is available yet.</summary>
    [JsonStringEnumMemberName("none")] None
}

/// <summary>Progress of an in-progress firmware update package download.</summary>
/// <param name="SpeedBytesPerSec">Current download speed, in bytes per second.</param>
/// <param name="ReceivedBytes">Bytes received so far.</param>
/// <param name="TotalBytes">Total download size, in bytes.</param>
public sealed record UpdateDownloadProgress(long? SpeedBytesPerSec, long? ReceivedBytes, long? TotalBytes);

/// <summary>Status of the firmware update/installation process.</summary>
/// <param name="IsAllowed">Whether update installation is currently allowed (e.g. the battery check has passed).</param>
/// <param name="Event">Most recent lifecycle event reported by the update process.</param>
/// <param name="Action">Step of the update process currently in progress.</param>
/// <param name="Status">Current or last result status.</param>
/// <param name="Detail">Optional free-form detail string for the current status.</param>
/// <param name="Download">Progress of an in-progress package download.</param>
public sealed record UpdateInstallStatus(
    bool? IsAllowed, UpdateEvent? Event, UpdateAction? Action,
    UpdateInstallResultStatus? Status, string? Detail, UpdateDownloadProgress? Download);

/// <summary>Status of the firmware update availability check.</summary>
/// <param name="AvailableVersion">Version of the available update, if any (empty if none).</param>
/// <param name="Event">Most recent lifecycle event reported by the check.</param>
/// <param name="Status">Result of the check.</param>
public sealed record UpdateCheckStatus(string? AvailableVersion, UpdateCheckEvent? Event, UpdateCheckResultStatus? Status);

/// <summary>Combined firmware update installation and update-check status.</summary>
/// <param name="Install">Status of the update/installation process.</param>
/// <param name="Check">Status of the update availability check.</param>
public sealed record UpdateStatus(UpdateInstallStatus? Install, UpdateCheckStatus? Check);

/// <summary>Firmware version to get the changelog for.</summary>
/// <param name="Version">Firmware version, e.g. <c>1.2.3</c>.</param>
public sealed record UpdateChangelogParams(string Version);

/// <summary>Changelog text for a firmware version.</summary>
/// <param name="Changelog">Changelog text.</param>
public sealed record UpdateChangelogResult(string? Changelog);

/// <summary>Firmware version to install.</summary>
/// <param name="Version">Firmware version, e.g. <c>1.2.3</c>. Must match a version previously reported as available.</param>
public sealed record UpdateInstallParams(string Version);

/// <summary>All fields are optional for POST requests; only provided fields are updated.</summary>
public sealed record AutoupdateSettings(bool? IsEnabled = null, string? IntervalStart = null, string? IntervalEnd = null);
