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
    private readonly bool _isCloud;
    private string? _token;
    private string? _httpAccessPassword;

    internal BusyBarTransport(HttpClient http, TimeSpan defaultTimeout, bool isCloud)
    {
        _http = http;
        _defaultTimeout = defaultTimeout;
        _isCloud = isCloud;
    }

    internal void SetToken(string? token) => _token = token;

    internal void SetHttpAccessPassword(string? password) => _httpAccessPassword = password;

    internal async Task<TResponse> SendJsonAsync<TResponse>(
        HttpMethod method, string path, IReadOnlyDictionary<string, string?>? query = null,
        object? jsonBody = null, RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        HttpContent? content = jsonBody is null ? null : JsonContent.Create(jsonBody, options: JsonOptions);
        var optionsToken = options?.CancellationToken ?? CancellationToken.None;
        var transportResponse = await SendCoreAsync(method, path, query, content, options, cancellationToken).ConfigureAwait(false);
        using var response = transportResponse.Response;
        try
        {
            return await transportResponse.ReadBodyAsync(async token =>
            {
                var stream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
                return (await JsonSerializer.DeserializeAsync<TResponse>(stream, JsonOptions, token).ConfigureAwait(false))!;
            }, cancellationToken, optionsToken).ConfigureAwait(false);
        }
        finally
        {
            // The body read above is fully awaited by this point, so it is now safe to retire the
            // timeout/linked CancellationTokenSources that backed ReadCancellationToken.
            transportResponse.DisposeCancellation();
        }
    }

    /// <summary>Sends a binary request body (ownership of <paramref name="requestBody"/> transfers to this call;
    /// it is disposed once the request completes) and parses a JSON response.</summary>
    internal async Task<TResponse> SendBinaryUploadAsync<TResponse>(
        HttpMethod method, string path, IReadOnlyDictionary<string, string?>? query,
        Stream requestBody, RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        var content = new StreamContent(requestBody);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        var optionsToken = options?.CancellationToken ?? CancellationToken.None;
        var transportResponse = await SendCoreAsync(method, path, query, content, options, cancellationToken).ConfigureAwait(false);
        using var response = transportResponse.Response;
        try
        {
            return await transportResponse.ReadBodyAsync(async token =>
            {
                var stream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
                return (await JsonSerializer.DeserializeAsync<TResponse>(stream, JsonOptions, token).ConfigureAwait(false))!;
            }, cancellationToken, optionsToken).ConfigureAwait(false);
        }
        finally
        {
            transportResponse.DisposeCancellation();
        }
    }

    internal async Task<Stream> SendBinaryDownloadAsync(
        HttpMethod method, string path, IReadOnlyDictionary<string, string?>? query = null,
        RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        var optionsToken = options?.CancellationToken ?? CancellationToken.None;
        var transportResponse = await SendCoreAsync(method, path, query, content: null, options, cancellationToken).ConfigureAwait(false);
        // Intentionally do NOT dispose the HttpResponseMessage here (matching the pre-existing behavior of this
        // method) — the returned Stream outlives this call and reads from the response's underlying connection.
        // However, ReadCancellationToken is only used by the ReadAsStreamAsync call below to obtain the Stream
        // object; nothing wires it into subsequent reads on the returned Stream. So, like SendJsonAsync and
        // SendBinaryUploadAsync, the timeout/linked CancellationTokenSources can (and must, to avoid leaking
        // registrations on the caller's token) be disposed once the stream has been obtained.
        try
        {
            return await transportResponse.ReadBodyAsync(
                token => transportResponse.Response.Content.ReadAsStreamAsync(token),
                cancellationToken, optionsToken).ConfigureAwait(false);
        }
        finally
        {
            transportResponse.DisposeCancellation();
        }
    }

    private async Task<TransportResponse> SendCoreAsync(
        HttpMethod method, string path, IReadOnlyDictionary<string, string?>? query,
        HttpContent? content, RequestOptions? options, CancellationToken cancellationToken)
    {
        var uri = BuildUri(path, query);
        // Deliberately excludes the query string (which may carry a device access key, see
        // BusyBar.Settings.AccessSetAsync) from anything that ends up in an exception message/log.
        var safeRequestPath = uri.GetLeftPart(UriPartial.Path);
        using var request = new HttpRequestMessage(method, uri) { Content = content };
        ApplyAuth(request);

        var timeout = options?.Timeout ?? _defaultTimeout;
        var optionsToken = options?.CancellationToken ?? CancellationToken.None;
        var timeoutCts = new CancellationTokenSource(timeout);
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken, optionsToken);

        HttpResponseMessage response;
        try
        {
            try
            {
                response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, linkedCts.Token)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested && !optionsToken.IsCancellationRequested)
            {
                throw new TimeoutException($"BUSY Bar request to {safeRequestPath} timed out after {timeout}.");
            }
        }
        catch
        {
            linkedCts.Dispose();
            timeoutCts.Dispose();
            throw;
        }

        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var statusCode = response.StatusCode;
                var reasonPhrase = response.ReasonPhrase;
                var body = await response.Content.ReadAsStringAsync(linkedCts.Token).ConfigureAwait(false);
                BusyBarErrorBody? parsed = null;
                try { parsed = JsonSerializer.Deserialize<BusyBarErrorBody>(body, JsonOptions); }
                catch (JsonException) { /* raw body still surfaced via RawBody */ }
                throw new BusyBarApiException(statusCode, reasonPhrase, body, parsed);
            }
            finally
            {
                response.Dispose();
                linkedCts.Dispose();
                timeoutCts.Dispose();
            }
        }

        return new TransportResponse(response, linkedCts.Token, timeoutCts, linkedCts, safeRequestPath, timeout);
    }

    private Uri BuildUri(string path, IReadOnlyDictionary<string, string?>? query)
    {
        var baseUri = _http.BaseAddress
            ?? throw new InvalidOperationException("BusyBar HttpClient has no BaseAddress configured.");
        var effectivePath = _isCloud ? path : StripCloudPathPrefix(path);
        var uri = new Uri(baseUri, effectivePath);
        if (query is not { Count: > 0 }) return uri;

        var parts = new List<string>();
        foreach (var (key, value) in query)
        {
            if (value is null) continue;
            parts.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
        }
        return new UriBuilder(uri) { Query = string.Join('&', parts) }.Uri;
    }

    /// <summary>
    /// Every path constant in this library is written in the cloud proxy's form (e.g. "busybar/status"),
    /// matching the vendored OpenAPI spec. A local device (USB/LAN) mounts the same endpoint under its own
    /// "/api/" base with this segment stripped entirely — confirmed against physical hardware. See
    /// <see cref="BusyBar.BuildBaseAddress"/> for the corresponding base-address handling.
    /// </summary>
    private static string StripCloudPathPrefix(string path)
    {
        const string cloudPrefix = "busybar/";
        return path.StartsWith(cloudPrefix, StringComparison.Ordinal) ? path[cloudPrefix.Length..] : path;
    }

    private void ApplyAuth(HttpRequestMessage request)
    {
        if (_token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        else if (_httpAccessPassword is not null)
            request.Headers.TryAddWithoutValidation("x-api-token", _httpAccessPassword);
    }

    /// <summary>
    /// Bundles a successful response with the <see cref="CancellationToken"/> that should govern reading its
    /// body — the same linked (timeout + caller + <see cref="RequestOptions.CancellationToken"/>) token used for
    /// the header phase — so timeout and cancellation continue to apply while the body is read.
    /// </summary>
    private readonly struct TransportResponse
    {
        internal HttpResponseMessage Response { get; }
        internal CancellationToken ReadCancellationToken { get; }
        private readonly CancellationTokenSource _timeoutCts;
        private readonly CancellationTokenSource _linkedCts;
        private readonly string _safeRequestPath;
        private readonly TimeSpan _timeout;

        internal TransportResponse(
            HttpResponseMessage response, CancellationToken readCancellationToken,
            CancellationTokenSource timeoutCts, CancellationTokenSource linkedCts,
            string safeRequestPath, TimeSpan timeout)
        {
            Response = response;
            ReadCancellationToken = readCancellationToken;
            _timeoutCts = timeoutCts;
            _linkedCts = linkedCts;
            _safeRequestPath = safeRequestPath;
            _timeout = timeout;
        }

        /// <summary>
        /// Disposes the timeout/linked <see cref="CancellationTokenSource"/>s backing
        /// <see cref="ReadCancellationToken"/>. Callers must only invoke this after they are done reading the
        /// response body — disposing earlier would invalidate the token mid-read.
        /// </summary>
        internal void DisposeCancellation()
        {
            _linkedCts.Dispose();
            _timeoutCts.Dispose();
        }

        /// <summary>
        /// Runs a body-read operation with <see cref="ReadCancellationToken"/> and, if it is canceled specifically
        /// because the per-request timeout elapsed (rather than <paramref name="callerToken"/> or
        /// <paramref name="optionsToken"/>), rethrows as <see cref="TimeoutException"/> — the same translation
        /// <c>SendCoreAsync</c> applies to the header-read phase, so a body that stalls past the timeout surfaces
        /// the same documented exception type as a stalled header response, and genuine caller cancellation
        /// (via either token) still surfaces as a plain <see cref="OperationCanceledException"/>.
        /// </summary>
        internal async Task<T> ReadBodyAsync<T>(
            Func<CancellationToken, Task<T>> readBody, CancellationToken callerToken, CancellationToken optionsToken)
        {
            try
            {
                return await readBody(ReadCancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (_timeoutCts.IsCancellationRequested && !callerToken.IsCancellationRequested && !optionsToken.IsCancellationRequested)
            {
                throw new TimeoutException($"BUSY Bar request to {_safeRequestPath} timed out after {_timeout}.");
            }
        }
    }
}
