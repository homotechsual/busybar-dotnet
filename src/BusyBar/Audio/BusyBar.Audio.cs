using System.Globalization;

namespace Busy.Bar;

public sealed partial class BusyBar
{
    /// <summary>Plays an audio file (e.g. a <c>.snd</c> file) from either the application's assets directory or the device's stock library.</summary>
    public Task<SuccessResponse> AudioPlayAsync(AudioPlayParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/audio/play",
            jsonBody: parameters, options: options, cancellationToken: cancellationToken);

    /// <summary>Stops any audio currently playing.</summary>
    public Task<SuccessResponse> AudioStopAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Delete, "busybar/audio/play",
            options: options, cancellationToken: cancellationToken);

    /// <summary>Retrieves the device's current audio volume.</summary>
    public Task<AudioVolumeInfo> AudioVolumeGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<AudioVolumeInfo>(HttpMethod.Get, "busybar/audio/volume",
            options: options, cancellationToken: cancellationToken);

    /// <summary>Sets the device's audio volume.</summary>
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
