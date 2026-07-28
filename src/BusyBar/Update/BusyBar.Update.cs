namespace Busy.Bar;

public sealed partial class BusyBar
{
    /// <summary>
    /// Uploads a firmware update package (TAR file) and initiates the update process; the device reboots once
    /// the update is applied. Ownership of <paramref name="firmwareTar"/> transfers to this call; it is disposed
    /// once the upload completes.
    /// </summary>
    public Task<SuccessResponse> UpdateFirmwareAsync(Stream firmwareTar, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendBinaryUploadAsync<SuccessResponse>(HttpMethod.Post, "busybar/update", query: null, requestBody: firmwareTar, options: options, cancellationToken: cancellationToken);

    /// <summary>Starts an asynchronous check for available firmware updates. Use <see cref="UpdateStatusGetAsync"/> to monitor progress.</summary>
    public Task<SuccessResponse> UpdateCheckAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/update/check", options: options, cancellationToken: cancellationToken);

    /// <summary>Gets the current firmware update installation and update-check status, including progress information.</summary>
    public Task<UpdateStatus> UpdateStatusGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<UpdateStatus>(HttpMethod.Get, "busybar/update/status", options: options, cancellationToken: cancellationToken);

    /// <summary>Gets the changelog for a specific firmware version.</summary>
    public Task<UpdateChangelogResult> UpdateChangelogGetAsync(UpdateChangelogParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<UpdateChangelogResult>(HttpMethod.Get, "busybar/update/changelog",
            query: new Dictionary<string, string?> { ["version"] = parameters.Version }, options: options, cancellationToken: cancellationToken);

    /// <summary>
    /// Starts asynchronous firmware installation of the given version from a remote URL. The update process
    /// (download, SHA verification, unpack, prepare, apply, reboot) runs in the background; use
    /// <see cref="UpdateStatusGetAsync"/> to monitor progress.
    /// </summary>
    public Task<SuccessResponse> UpdateInstallAsync(UpdateInstallParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/update/install",
            query: new Dictionary<string, string?> { ["version"] = parameters.Version }, options: options, cancellationToken: cancellationToken);

    /// <summary>Signals the updater to abort an ongoing firmware package download.</summary>
    public Task<SuccessResponse> UpdateAbortDownloadAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/update/abort_download", options: options, cancellationToken: cancellationToken);

    /// <summary>Gets the device's current automatic-update configuration.</summary>
    public Task<AutoupdateSettings> UpdateAutoupdateGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<AutoupdateSettings>(HttpMethod.Get, "busybar/update/autoupdate", options: options, cancellationToken: cancellationToken);

    /// <summary>Sets the device's automatic-update configuration. Only the fields provided in <paramref name="settings"/> are updated.</summary>
    public Task<SuccessResponse> UpdateAutoupdateSetAsync(AutoupdateSettings settings, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/update/autoupdate", jsonBody: settings, options: options, cancellationToken: cancellationToken);
}
