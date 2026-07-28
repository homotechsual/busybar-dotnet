namespace Busy.Bar;

/// <summary>Identifies where an uploaded asset should be stored.</summary>
/// <param name="ApplicationName">Application ID whose assets directory the file is uploaded into.</param>
/// <param name="File">Filename to store the uploaded asset under.</param>
public sealed record AssetsUploadParams(string ApplicationName, string File);

/// <summary>Identifies whose assets should be deleted.</summary>
/// <param name="ApplicationName">Application ID whose assets should all be deleted.</param>
public sealed record AssetsDeleteParams(string ApplicationName);
