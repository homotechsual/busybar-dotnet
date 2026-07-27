using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Busy.Bar.Internal;

internal sealed class BusyBarTransport
{
    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) }
    };

    private readonly HttpClient _http;
    private readonly TimeSpan _defaultTimeout;
    private string? _token;
    private string? _httpAccessPassword;

    internal BusyBarTransport(HttpClient http, TimeSpan defaultTimeout)
    {
        _http = http;
        _defaultTimeout = defaultTimeout;
    }

    internal void SetToken(string? token) => _token = token;

    internal void SetHttpAccessPassword(string? password) => _httpAccessPassword = password;

    internal async Task<TResponse> SendJsonAsync<TResponse>(
        HttpMethod method, string path, IReadOnlyDictionary<string, string?>? query = null,
        object? jsonBody = null, RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        HttpContent? content = jsonBody is null ? null : JsonContent.Create(jsonBody, options: JsonOptions);
        using var response = await SendCoreAsync(method, path, query, content, options, cancellationToken).ConfigureAwait(false);
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        return (await JsonSerializer.DeserializeAsync<TResponse>(stream, JsonOptions, cancellationToken).ConfigureAwait(false))!;
    }

    /// <summary>Sends a binary request body (ownership of <paramref name="requestBody"/> transfers to this call;
    /// it is disposed once the request completes) and parses a JSON response.</summary>
    internal async Task<TResponse> SendBinaryUploadAsync<TResponse>(
        HttpMethod method, string path, IReadOnlyDictionary<string, string?>? query,
        Stream requestBody, RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        var content = new StreamContent(requestBody);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        using var response = await SendCoreAsync(method, path, query, content, options, cancellationToken).ConfigureAwait(false);
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        return (await JsonSerializer.DeserializeAsync<TResponse>(stream, JsonOptions, cancellationToken).ConfigureAwait(false))!;
    }

    internal async Task<Stream> SendBinaryDownloadAsync(
        HttpMethod method, string path, IReadOnlyDictionary<string, string?>? query = null,
        RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        var response = await SendCoreAsync(method, path, query, content: null, options, cancellationToken).ConfigureAwait(false);
        return await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task<HttpResponseMessage> SendCoreAsync(
        HttpMethod method, string path, IReadOnlyDictionary<string, string?>? query,
        HttpContent? content, RequestOptions? options, CancellationToken cancellationToken)
    {
        var uri = BuildUri(path, query);
        using var request = new HttpRequestMessage(method, uri) { Content = content };
        ApplyAuth(request);

        var timeout = options?.Timeout ?? _defaultTimeout;
        var callerToken = options?.CancellationToken ?? cancellationToken;
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, callerToken);

        HttpResponseMessage response;
        try
        {
            response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, linkedCts.Token)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !callerToken.IsCancellationRequested)
        {
            throw new TimeoutException($"BUSY Bar request to {uri} timed out after {timeout}.");
        }

        if (!response.IsSuccessStatusCode)
        {
            var statusCode = response.StatusCode;
            var reasonPhrase = response.ReasonPhrase;
            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            BusyBarErrorBody? parsed = null;
            try { parsed = JsonSerializer.Deserialize<BusyBarErrorBody>(body, JsonOptions); }
            catch (JsonException) { /* raw body still surfaced via RawBody */ }
            response.Dispose();
            throw new BusyBarApiException(statusCode, reasonPhrase, body, parsed);
        }

        return response;
    }

    private Uri BuildUri(string path, IReadOnlyDictionary<string, string?>? query)
    {
        var baseUri = _http.BaseAddress
            ?? throw new InvalidOperationException("BusyBar HttpClient has no BaseAddress configured.");
        var uri = new Uri(baseUri, path);
        if (query is not { Count: > 0 }) return uri;

        var parts = new List<string>();
        foreach (var (key, value) in query)
        {
            if (value is null) continue;
            parts.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
        }
        return new UriBuilder(uri) { Query = string.Join('&', parts) }.Uri;
    }

    private void ApplyAuth(HttpRequestMessage request)
    {
        if (_token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        else if (_httpAccessPassword is not null)
            request.Headers.TryAddWithoutValidation("x-api-token", _httpAccessPassword);
    }
}
