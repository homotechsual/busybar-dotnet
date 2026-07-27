namespace Busy.Bar;

public sealed partial class BusyBar
{
    /// <summary>Ownership of <paramref name="fileContent"/> transfers to this call; it is disposed once the upload completes.</summary>
    public Task<SuccessResponse> StorageWriteAsync(StorageWriteParams parameters, Stream fileContent, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendBinaryUploadAsync<SuccessResponse>(HttpMethod.Post, "busybar/storage/write",
            query: new Dictionary<string, string?> { ["path"] = parameters.Path },
            requestBody: fileContent, options: options, cancellationToken: cancellationToken);

    public Task<Stream> StorageReadAsync(StorageReadParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendBinaryDownloadAsync(HttpMethod.Get, "busybar/storage/read",
            query: new Dictionary<string, string?> { ["path"] = parameters.Path }, options: options, cancellationToken: cancellationToken);

    public Task<StorageList> StorageListAsync(StorageListParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<StorageList>(HttpMethod.Get, "busybar/storage/list",
            query: new Dictionary<string, string?> { ["path"] = parameters.Path }, options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> StorageRemoveAsync(StorageRemoveParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Delete, "busybar/storage/remove",
            query: new Dictionary<string, string?> { ["path"] = parameters.Path }, options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> StorageMkdirAsync(StorageMkdirParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/storage/mkdir",
            query: new Dictionary<string, string?> { ["path"] = parameters.Path }, options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> StorageRenameAsync(StorageRenameParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/storage/rename",
            query: new Dictionary<string, string?> { ["path"] = parameters.Path, ["new_path"] = parameters.NewPath },
            options: options, cancellationToken: cancellationToken);

    public Task<StorageStatus> StorageStatusGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<StorageStatus>(HttpMethod.Get, "busybar/storage/status", options: options, cancellationToken: cancellationToken);
}
