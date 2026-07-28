namespace Busy.Bar;

public sealed partial class BusyBar
{
    /// <summary>Gets the device's local HTTP API version.</summary>
    public Task<VersionInfo> SystemVersionGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<VersionInfo>(HttpMethod.Get, "busybar/version", options: options, cancellationToken: cancellationToken);

    /// <summary>Gets the device's current network connection type (USB or Wi-Fi).</summary>
    public Task<NetworkInterfaceInfo> SystemTransportGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<NetworkInterfaceInfo>(HttpMethod.Get, "busybar/transport", options: options, cancellationToken: cancellationToken);

    /// <summary>Gets combined device, firmware, system, and power status.</summary>
    public Task<SystemStatus> SystemStatusGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<SystemStatus>(HttpMethod.Get, "busybar/status", options: options, cancellationToken: cancellationToken);

    /// <summary>Gets hardware identity and provisioning information for the device.</summary>
    public Task<StatusDevice> SystemStatusDeviceGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<StatusDevice>(HttpMethod.Get, "busybar/status/device", options: options, cancellationToken: cancellationToken);

    /// <summary>Gets firmware build and version information for the device.</summary>
    public Task<StatusFirmware> SystemStatusFirmwareGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<StatusFirmware>(HttpMethod.Get, "busybar/status/firmware", options: options, cancellationToken: cancellationToken);

    /// <summary>Gets general system status information.</summary>
    public Task<StatusSystem> SystemStatusSystemGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<StatusSystem>(HttpMethod.Get, "busybar/status/system", options: options, cancellationToken: cancellationToken);

    /// <summary>Gets power and battery status.</summary>
    public Task<StatusPower> SystemStatusPowerGetAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _transport.SendJsonAsync<StatusPower>(HttpMethod.Get, "busybar/status/power", options: options, cancellationToken: cancellationToken);

    /// <summary>Snapshots the device's in-memory log buffer to a file on internal storage.</summary>
    public Task<LogDumpResult> SystemLogDumpAsync(LogDumpParams? parameters = null, RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = parameters?.Filename is { } filename
            ? new Dictionary<string, string?> { ["filename"] = filename }
            : null;
        return _transport.SendJsonAsync<LogDumpResult>(HttpMethod.Post, "busybar/log_dump", query: query, options: options, cancellationToken: cancellationToken);
    }
}
