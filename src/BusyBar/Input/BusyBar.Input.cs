namespace Busy.Bar;

public sealed partial class BusyBar
{
    /// <summary>Simulates a single key press event on the device.</summary>
    public Task<SuccessResponse> InputKeySetAsync(InputKeyParams parameters, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SuccessResponse>(HttpMethod.Post, "busybar/input",
            query: new Dictionary<string, string?> { ["key"] = ToApiString(parameters.Key) },
            options: options, cancellationToken: cancellationToken);

    private static string ToApiString(InputKey key) => key switch
    {
        InputKey.Up => "up",
        InputKey.Down => "down",
        InputKey.Ok => "ok",
        InputKey.Back => "back",
        InputKey.Start => "start",
        InputKey.Busy => "busy",
        InputKey.Custom => "custom",
        InputKey.Off => "off",
        InputKey.Apps => "apps",
        InputKey.Settings => "settings",
        _ => throw new ArgumentOutOfRangeException(nameof(key))
    };
}
