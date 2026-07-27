using System.Globalization;

namespace Busy.Bar;

public sealed partial class BusyBar
{
    public Task<SuccessResponse> AudioPlayAsync(AudioPlayParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/audio/play",
            jsonBody: parameters, options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> AudioStopAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Delete, "busybar/audio/play",
            options: options, cancellationToken: cancellationToken);

    public Task<AudioVolumeInfo> AudioVolumeGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<AudioVolumeInfo>(HttpMethod.Get, "busybar/audio/volume",
            options: options, cancellationToken: cancellationToken);

    public Task<SuccessResponse> AudioVolumeSetAsync(AudioVolumeSetParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string?>
        {
            ["volume"] = parameters.Volume.ToString(CultureInfo.InvariantCulture)
        };
        if (parameters.Silent is { } silent) query["silent"] = silent.ToString(CultureInfo.InvariantCulture);

        return _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/audio/volume",
            query: query, options: options, cancellationToken: cancellationToken);
    }
}
