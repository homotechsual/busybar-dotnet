namespace Busy.Bar;

public sealed partial class BusyBar
{
    public Task<HttpAccessInfo> AccessGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<HttpAccessInfo>(HttpMethod.Get, "busybar/access", options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> AccessSetAsync(AccessSetParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string?> { ["mode"] = ToApiString(parameters.Mode) };
        if (parameters.Key is not null) query["key"] = parameters.Key;
        return _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/access", query: query, options: options, cancellationToken: cancellationToken);
    }

    public Task<NameInfo> NameGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<NameInfo>(HttpMethod.Get, "busybar/name", options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> NameSetAsync(NameInfo parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/name", jsonBody: parameters, options: options, cancellationToken: cancellationToken);

    private static string ToApiString(HttpAccessMode mode) => mode switch
    {
        HttpAccessMode.Disabled => "disabled",
        HttpAccessMode.Enabled => "enabled",
        HttpAccessMode.Key => "key",
        _ => throw new ArgumentOutOfRangeException(nameof(mode))
    };
}
