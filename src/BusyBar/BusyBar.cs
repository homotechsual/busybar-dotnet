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
        _transport = new Internal.BusyBarTransport(_http, options.Timeout);
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
        _transport = new Internal.BusyBarTransport(_http, options.Timeout);
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

    /// <summary>Disposes the underlying <see cref="HttpClient"/>, but only if this instance created it
    /// (the <see cref="BusyBar(BusyBarOptions)"/> overload) — a caller-supplied client
    /// (<see cref="BusyBar(HttpClient, BusyBarOptions)"/>) is left untouched.</summary>
    public void Dispose()
    {
        if (_ownsHttpClient) _http.Dispose();
    }

    private static Uri BuildBaseAddress(string addr)
    {
        if (Uri.TryCreate(addr, UriKind.Absolute, out var absolute)
            && (absolute.Scheme == Uri.UriSchemeHttp || absolute.Scheme == Uri.UriSchemeHttps))
        {
            return EnsureTrailingSlash(absolute);
        }

        var scheme = addr.Equals("api.busy.app", StringComparison.OrdinalIgnoreCase) ? "https" : "http";
        return EnsureTrailingSlash(new Uri($"{scheme}://{addr}"));
    }

    private static Uri EnsureTrailingSlash(Uri uri)
        => uri.AbsoluteUri.EndsWith('/') ? uri : new Uri(uri.AbsoluteUri + "/");
}
