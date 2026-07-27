namespace Busy.Bar;

/// <summary>
/// Connection options for <see cref="BusyBar"/>. Mirrors the constructor options of the
/// official <c>@busy-app/busy-lib</c> TypeScript client.
/// </summary>
public sealed class BusyBarOptions
{
    /// <summary>
    /// IP address, hostname, or full URL of the BUSY Bar. Defaults to the USB-Ethernet
    /// address <c>10.0.4.20</c>. If no scheme is given, <c>http://</c> is assumed, except
    /// for the cloud proxy host <c>api.busy.app</c>, which defaults to <c>https://</c>.
    /// </summary>
    public string Addr { get; init; } = "10.0.4.20";

    /// <summary>Bearer token for the <c>api.busy.app</c> cloud proxy.</summary>
    public string? Token { get; init; }

    /// <summary>HTTP access password, sent as the <c>x-api-token</c> header, for LAN Wi-Fi access.</summary>
    public string? HttpAccessPassword { get; init; }

    /// <summary>Default per-request timeout. Overridable per call via <see cref="RequestOptions.Timeout"/>.</summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(3);
}
