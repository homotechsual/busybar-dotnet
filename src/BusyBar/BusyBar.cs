namespace Busy.Bar;

/// <summary>
/// A typed client for the BUSY Bar HTTP API. Aggregates every namespace
/// (Account, Assets, Audio, Ble, Busy, Display, Input, Settings, SmartHome, Storage,
/// System, Time, Update, Wifi) on one instance, mirroring <c>@busy-app/busy-lib</c>.
/// </summary>
public sealed partial class BusyBar : IDisposable
{
    private readonly Internal.BusyBarTransport _transport;
    private readonly HttpClient _http;
    private readonly bool _ownsHttpClient;

    /// <summary>Connects using the default options (<c>10.0.4.20</c> over USB-Ethernet).</summary>
    public BusyBar() : this(new BusyBarOptions())
    {
    }

    /// <summary>Connects using the given options.</summary>
    public BusyBar(BusyBarOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Addr);

        _http = new HttpClient { BaseAddress = BuildBaseAddress(options.Addr) };
        // BusyBarTransport owns per-request timeout enforcement (RequestOptions.Timeout / BusyBarOptions.Timeout).
        // HttpClient's own default Timeout (100s) must be disabled so it never fires first — if it did, it would
        // throw a bare TaskCanceledException instead of the documented TimeoutException, and it would silently
        // cap any per-call timeout configured above 100 seconds.
        _http.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
        _ownsHttpClient = true;
        _transport = new Internal.BusyBarTransport(_http, options.Timeout, IsCloudHost(_http.BaseAddress!));
        ApplyInitialAuth(options);
    }

    /// <summary>
    /// Connects using a caller-supplied, already-configured <see cref="HttpClient"/> — e.g. one obtained from
    /// <c>IHttpClientFactory</c>, or with custom <see cref="HttpMessageHandler"/>s for retry, logging, or
    /// proxying. Use this overload instead of <see cref="BusyBar(BusyBarOptions)"/> when you need control over
    /// connection pooling/lifetime or want to compose the client with other handlers. This constructor does NOT
    /// take ownership of <paramref name="httpClient"/>: <see cref="Dispose"/> never disposes it, so the caller
    /// remains responsible for its lifetime.
    /// </summary>
    public BusyBar(HttpClient httpClient, BusyBarOptions options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);
        httpClient.BaseAddress ??= BuildBaseAddress(options.Addr);
        _http = httpClient;
        _ownsHttpClient = false;
        _transport = new Internal.BusyBarTransport(_http, options.Timeout, IsCloudHost(_http.BaseAddress!));
        ApplyInitialAuth(options);
    }

    private void ApplyInitialAuth(BusyBarOptions options)
    {
        if (options.Token is not null) _transport.SetToken(options.Token);
        if (options.HttpAccessPassword is not null) _transport.SetHttpAccessPassword(options.HttpAccessPassword);
    }

    /// <summary>Sets or replaces the cloud proxy bearer token at runtime.</summary>
    public void SetToken(string token) => _transport.SetToken(token);

    /// <summary>Sets or replaces the LAN HTTP access password at runtime.</summary>
    public void SetHttpAccessPassword(string password) => _transport.SetHttpAccessPassword(password);

    /// <summary>
    /// Calls an arbitrary BUSY Bar HTTP API endpoint and deserializes its JSON response as
    /// <typeparamref name="TResponse"/>. Goes through the exact same auth, base-address resolution,
    /// timeout/cancellation, and error handling (<see cref="BusyBarApiException"/> on a non-2xx response) as
    /// every typed method on this class — it's an escape hatch for endpoints this library doesn't wrap yet
    /// (e.g. a newer firmware feature), not a bypass of that machinery. <paramref name="path"/> follows the
    /// same convention as every other call: write it in the cloud proxy's form (e.g. <c>"busybar/status"</c>)
    /// regardless of whether you're actually talking to a local device or the cloud proxy.
    /// </summary>
    public Task<TResponse> InvokeAsync<TResponse>(
        HttpMethod method, string path, IReadOnlyDictionary<string, string?>? query = null,
        object? jsonBody = null, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<TResponse>(method, path, query, jsonBody, options, cancellationToken);

    /// <summary>Disposes the underlying <see cref="HttpClient"/>, but only if this instance created it
    /// (the <see cref="BusyBar(BusyBarOptions)"/> overload) — a caller-supplied client
    /// (<see cref="BusyBar(HttpClient, BusyBarOptions)"/>) is left untouched.</summary>
    public void Dispose()
    {
        if (_ownsHttpClient) _http.Dispose();
    }

    /// <summary>
    /// Resolves the given address to a base <see cref="Uri"/>, appending the device's fixed API mount point.
    /// Note: if <paramref name="addr"/> is a full URL with a path (e.g. "http://192.168.1.5:8080/somepath"), that
    /// path is deliberately discarded — the device's API mount point is fixed by the device itself, not
    /// caller-configurable.
    /// </summary>
    private static Uri BuildBaseAddress(string addr)
    {
        string authority;
        if (Uri.TryCreate(addr, UriKind.Absolute, out var absolute)
            && (absolute.Scheme == Uri.UriSchemeHttp || absolute.Scheme == Uri.UriSchemeHttps))
        {
            authority = absolute.GetLeftPart(UriPartial.Authority);
        }
        else
        {
            var scheme = addr.Equals("api.busy.app", StringComparison.OrdinalIgnoreCase) ? "https" : "http";
            authority = $"{scheme}://{addr}";
        }

        var isCloud = IsCloudHost(new Uri(authority));
        var pathSuffix = isCloud ? "/" : "/api/";
        return new Uri(authority + pathSuffix);
    }

    /// <summary>
    /// True only for the BUSY Cloud proxy host. The vendored OpenAPI spec's paths (e.g. "busybar/status")
    /// describe that proxy's namespace; a local device (USB or LAN) mounts the same endpoints directly under
    /// its own "/api/" base with the "busybar/" segment stripped — confirmed against physical hardware. See
    /// <see cref="Internal.BusyBarTransport"/>'s path handling, which strips that segment for local hosts.
    /// </summary>
    private static bool IsCloudHost(Uri baseAddress) =>
        baseAddress.Host.Equals("api.busy.app", StringComparison.OrdinalIgnoreCase);
}
