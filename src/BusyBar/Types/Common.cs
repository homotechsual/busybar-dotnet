namespace Busy.Bar;

/// <summary>Generic success acknowledgement returned by most mutating endpoints.</summary>
public sealed record SuccessResponse(string Result);

/// <summary>Body of a non-2xx JSON error response.</summary>
public sealed record BusyBarErrorBody(string Error, int? Code);
