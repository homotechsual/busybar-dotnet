namespace Busy.Bar;

public sealed record AssetsUploadParams(string ApplicationName, string File);

public sealed record AssetsDeleteParams(string ApplicationName);
