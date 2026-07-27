namespace Busy.Bar;

/// <summary>Exactly one of <see cref="Path"/> or <see cref="StockPath"/> must be set.</summary>
public sealed record AudioPlayParams(string ApplicationName, string? Path = null, string? StockPath = null);

public sealed record AudioVolumeInfo(double? Volume);

/// <summary><paramref name="Silent"/>: 0 plays the volume-change sound (default), 1 stays silent.</summary>
public sealed record AudioVolumeSetParams(double Volume, int? Silent = null);
