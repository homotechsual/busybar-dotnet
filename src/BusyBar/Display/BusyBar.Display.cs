using System.Globalization;

namespace Busy.Bar;

public sealed partial class BusyBar
{
    public Task<SuccessResponse> DisplayDrawAsync(DisplayDrawParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/display/draw", jsonBody: parameters, options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> DisplayClearAsync(DisplayClearParams? parameters = null, RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = parameters?.ApplicationName is { } appName
            ? new Dictionary<string, string?> { ["application_name"] = appName }
            : null;
        return _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Delete, "busybar/display/draw", query: query, options: options, cancellationToken: cancellationToken);
    }

    public Task<Stream> DisplayScreenFrameGetAsync(ScreenFrameGetParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendBinaryDownloadAsync(HttpMethod.Get, "busybar/screen",
            query: new Dictionary<string, string?> { ["display"] = parameters.Display.ToString(CultureInfo.InvariantCulture) },
            options: options, cancellationToken: cancellationToken);

    public Task<DisplayBrightnessInfo> DisplayBrightnessGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<DisplayBrightnessInfo>(HttpMethod.Get, "busybar/display/brightness", options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> DisplayBrightnessSetAsync(DisplayBrightnessParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/display/brightness",
            query: new Dictionary<string, string?> { ["value"] = parameters.Value }, options: options, cancellationToken: cancellationToken);
}
