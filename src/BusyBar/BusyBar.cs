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
        _ownsHttpClient = true;
        _transport = new Internal.BusyBarTransport(_http, options.Timeout);
        ApplyInitialAuth(options);
    }

    /// <summary>Test-only constructor allowing injection of a fake <see cref="HttpMessageHandler"/>. The
    /// supplied <paramref name="httpClient"/> is never disposed by <see cref="Dispose"/> — the caller owns it.</summary>
    internal BusyBar(HttpClient httpClient, BusyBarOptions options)
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
    /// (the constructor's <c>options</c> overload) — a caller-supplied client (test constructor) is left untouched.</summary>
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
