using System.Net;
using System.Text;

namespace BusyBar.Tests.Internal;

internal sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }
    public string? LastRequestBody { get; private set; }
    public HttpStatusCode ResponseStatusCode { get; set; } = HttpStatusCode.OK;
    public string ResponseBody { get; set; } = "{}";
    public string ResponseContentType { get; set; } = "application/json";
    public TimeSpan? ResponseDelay { get; set; }

    /// <summary>
    /// When set, headers are returned immediately (as <see cref="HttpCompletionOption.ResponseHeadersRead"/>
    /// callers expect) but the response body content only becomes readable after this delay — simulating a
    /// device that acknowledges a request promptly and then stalls while streaming the body. Distinct from
    /// <see cref="ResponseDelay"/>, which delays before headers are even sent.
    /// </summary>
    public TimeSpan? BodyDelay { get; set; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;
        LastRequestBody = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
        if (ResponseDelay is { } delay)
            await Task.Delay(delay, cancellationToken);

        HttpContent content = BodyDelay is { } bodyDelay
            ? new DelayedContent(ResponseBody, Encoding.UTF8, ResponseContentType, bodyDelay)
            : new StringContent(ResponseBody, Encoding.UTF8, ResponseContentType);

        return new HttpResponseMessage(ResponseStatusCode) { Content = content };
    }

    /// <summary>An <see cref="HttpContent"/> whose body only becomes available after a delay, so it can observe
    /// cancellation of the token passed to the read call (e.g. <c>ReadAsStreamAsync</c>/<c>CopyToAsync</c>)
    /// independently of whatever token governed the initial <c>SendAsync</c>.</summary>
    private sealed class DelayedContent : HttpContent
    {
        private readonly byte[] _bytes;
        private readonly TimeSpan _delay;

        public DelayedContent(string body, Encoding encoding, string mediaType, TimeSpan delay)
        {
            _bytes = encoding.GetBytes(body);
            _delay = delay;
            Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType) { CharSet = encoding.WebName };
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
            => await SerializeToStreamAsync(stream, context, CancellationToken.None);

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
        {
            await Task.Delay(_delay, cancellationToken);
            await stream.WriteAsync(_bytes, cancellationToken);
        }

        // HttpContent's default ReadAsStreamAsync(CancellationToken) buffering path does not reliably forward the
        // token into SerializeToStreamAsync, so override the stream-creation hook directly to guarantee the delay
        // observes whatever token the caller passed to ReadAsStreamAsync.
        protected override async Task<Stream> CreateContentReadStreamAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(_delay, cancellationToken);
            return new MemoryStream(_bytes);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _bytes.Length;
            return true;
        }
    }
}
