using System.Net;

namespace Busy.Bar;

/// <summary>Thrown when the BUSY Bar responds with a non-2xx HTTP status.</summary>
public sealed class BusyBarApiException : Exception
{
    /// <summary>The response's HTTP status code.</summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>The response's HTTP reason phrase, if any.</summary>
    public string? ReasonPhrase { get; }

    /// <summary>The raw response body text, regardless of whether it could be parsed as JSON.</summary>
    public string RawBody { get; }

    /// <summary>The response body parsed as a <see cref="BusyBarErrorBody"/>, or <see langword="null"/> if it wasn't valid JSON in that shape.</summary>
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
