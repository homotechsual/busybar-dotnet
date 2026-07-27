namespace Busy.Bar;

public sealed partial class BusyBar
{
    /// <summary>Ownership of <paramref name="firmwareTar"/> transfers to this call; it is disposed once the upload completes.</summary>
    public Task<SuccessResponse> UpdateFirmwareAsync(Stream firmwareTar, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendBinaryUploadAsync<SuccessResponse>(HttpMethod.Post, "busybar/update", query: null, requestBody: firmwareTar, options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> UpdateCheckAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/update/check", options: options, cancellationToken: cancellationToken);

    public Task<UpdateStatus> UpdateStatusGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<UpdateStatus>(HttpMethod.Get, "busybar/update/status", options: options, cancellationToken: cancellationToken);

    public Task<UpdateChangelogResult> UpdateChangelogGetAsync(UpdateChangelogParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<UpdateChangelogResult>(HttpMethod.Get, "busybar/update/changelog",
            query: new Dictionary<string, string?> { ["version"] = parameters.Version }, options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> UpdateInstallAsync(UpdateInstallParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/update/install",
            query: new Dictionary<string, string?> { ["version"] = parameters.Version }, options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> UpdateAbortDownloadAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/update/abort_download", options: options, cancellationToken: cancellationToken);

    public Task<AutoupdateSettings> UpdateAutoupdateGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<AutoupdateSettings>(HttpMethod.Get, "busybar/update/autoupdate", options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> UpdateAutoupdateSetAsync(AutoupdateSettings settings, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/update/autoupdate", jsonBody: settings, options: options, cancellationToken: cancellationToken);
}
