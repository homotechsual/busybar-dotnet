namespace Busy.Bar;

public sealed partial class BusyBar
{
    /// <summary>Gets the device's HTTP API access over Wi-Fi configuration.</summary>
    public Task<HttpAccessInfo> AccessGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<HttpAccessInfo>(HttpMethod.Get, "busybar/access", options: options, cancellationToken: cancellationToken);

    /// <summary>Sets the device's HTTP API access over Wi-Fi configuration.</summary>
    public Task<SuccessResponse> AccessSetAsync(AccessSetParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string?> { ["mode"] = ToApiString(parameters.Mode) };
        if (parameters.Key is not null) query["key"] = parameters.Key;
        return _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/access", query: query, options: options, cancellationToken: cancellationToken);
    }

    /// <summary>Gets the device's current display name.</summary>
    public Task<NameInfo> NameGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<NameInfo>(HttpMethod.Get, "busybar/name", options: options, cancellationToken: cancellationToken);

    /// <summary>Sets the device's display name.</summary>
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
