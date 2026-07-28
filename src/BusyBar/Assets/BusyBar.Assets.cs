namespace Busy.Bar;

public sealed partial class BusyBar
{
    /// <summary>Ownership of <paramref name="fileContent"/> transfers to this call; it is disposed once the upload completes.</summary>
    public Task<SuccessResponse> AssetsUploadAsync(AssetsUploadParams parameters, Stream fileContent, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendBinaryUploadAsync<SuccessResponse>(HttpMethod.Post, "busybar/assets/upload",
            query: new Dictionary<string, string?>
            {
                ["application_name"] = parameters.ApplicationName,
                ["file"] = parameters.File
            },
            requestBody: fileContent, options: options, cancellationToken: cancellationToken);

    /// <summary>Deletes all uploaded assets for the given application.</summary>
    public Task<SuccessResponse> AssetsDeleteAsync(AssetsDeleteParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Delete, "busybar/assets/upload",
            query: new Dictionary<string, string?> { ["application_name"] = parameters.ApplicationName },
            options: options, cancellationToken: cancellationToken);
}
