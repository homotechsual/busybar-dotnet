using System.Globalization;

namespace Busy.Bar;

public sealed partial class BusyBar
{
    /// <summary>Draws one or more elements on a display.</summary>
    public Task<SuccessResponse> DisplayDrawAsync(DisplayDrawParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/display/draw", jsonBody: parameters, options: options, cancellationToken: cancellationToken);

    /// <summary>Clears display elements previously drawn by the Canvas application. If <paramref name="parameters"/> specifies an application name, only that application's elements are removed.</summary>
    public Task<SuccessResponse> DisplayClearAsync(DisplayClearParams? parameters = null, RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = parameters?.ApplicationName is { } appName
            ? new Dictionary<string, string?> { ["application_name"] = appName }
            : null;
        return _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Delete, "busybar/display/draw", query: query, options: options, cancellationToken: cancellationToken);
    }

    /// <summary>Captures a single frame from the requested display, as a BMP image stream.</summary>
    /// <returns>A stream containing the raw BMP image data. The caller owns the returned stream and must dispose it.</returns>
    public Task<Stream> DisplayScreenFrameGetAsync(ScreenFrameGetParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendBinaryDownloadAsync(HttpMethod.Get, "busybar/screen",
            query: new Dictionary<string, string?> { ["display"] = parameters.Display.ToString(CultureInfo.InvariantCulture) },
            options: options, cancellationToken: cancellationToken);

    /// <summary>Retrieves the device's current display brightness.</summary>
    public Task<DisplayBrightnessInfo> DisplayBrightnessGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<DisplayBrightnessInfo>(HttpMethod.Get, "busybar/display/brightness", options: options, cancellationToken: cancellationToken);

    /// <summary>Sets the device's display brightness, for one or both displays.</summary>
    public Task<SuccessResponse> DisplayBrightnessSetAsync(DisplayBrightnessParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/display/brightness",
            query: new Dictionary<string, string?> { ["value"] = parameters.Value }, options: options, cancellationToken: cancellationToken);
}
