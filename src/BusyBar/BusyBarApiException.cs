using System.Net;

namespace Busy.Bar;

/// <summary>Thrown when the BUSY Bar responds with a non-2xx HTTP status.</summary>
public sealed class BusyBarApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? ReasonPhrase { get; }
    public string RawBody { get; }
    public BusyBarErrorBody? ErrorBody { get; }

    internal BusyBarApiException(HttpStatusCode statusCode, string? reasonPhrase, string rawBody, BusyBarErrorBody? errorBody)
        : base(BuildMessage(statusCode, reasonPhrase, errorBody))
    {
        StatusCode = statusCode;
        ReasonPhrase = reasonPhrase;
        RawBody = rawBody;
        ErrorBody = errorBody;
    }

    private static string BuildMessage(HttpStatusCode statusCode, string? reasonPhrase, BusyBarErrorBody? errorBody)
        => errorBody is not null
            ? $"BUSY Bar API returned {(int)statusCode} {reasonPhrase}: {errorBody.Error}"
            : $"BUSY Bar API returned {(int)statusCode} {reasonPhrase}";
}
