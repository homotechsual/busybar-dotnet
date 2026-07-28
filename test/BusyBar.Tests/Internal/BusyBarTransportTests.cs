using Busy.Bar;
using Busy.Bar.Internal;
using BusyBar.Tests.Internal;
using Xunit;

namespace BusyBar.Tests.Internal;

public class BusyBarTransportTests
{
    private static (BusyBarTransport transport, FakeHttpMessageHandler handler) CreateTransport(TimeSpan? defaultTimeout = null, bool isCloud = false)
    {
        var handler = new FakeHttpMessageHandler();
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        return (new BusyBarTransport(http, defaultTimeout ?? TimeSpan.FromSeconds(3), isCloud), handler);
    }

    [Fact]
    public async Task SendJsonAsync_SendsQueryStringAndParsesResponse()
    {
        var (transport, handler) = CreateTransport();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        var result = await transport.SendJsonAsync<SuccessResponse>(
            HttpMethod.Post, "busybar/name",
            query: new Dictionary<string, string?> { ["value"] = "50" });

        Assert.Equal("OK", result.Result);
        Assert.EndsWith("name?value=50", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task SendJsonAsync_SendsJsonBodyWithSnakeCaseProperties()
    {
        var (transport, handler) = CreateTransport();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await transport.SendJsonAsync<SuccessResponse>(
            HttpMethod.Post, "busybar/name", jsonBody: new { DeviceName = "My Bar" });

        Assert.Contains("\"device_name\":\"My Bar\"", handler.LastRequestBody);
    }

    [Fact]
    public async Task SendJsonAsync_UsesBearerToken_WhenTokenSet()
    {
        var (transport, handler) = CreateTransport();
        handler.ResponseBody = "{\"result\":\"OK\"}";
        transport.SetToken("abc123");

        await transport.SendJsonAsync<SuccessResponse>(HttpMethod.Get, "busybar/version");

        Assert.Equal("Bearer", handler.LastRequest!.Headers.Authorization!.Scheme);
        Assert.Equal("abc123", handler.LastRequest!.Headers.Authorization!.Parameter);
    }

    [Fact]
    public async Task SendJsonAsync_UsesXApiTokenHeader_WhenHttpAccessPasswordSet()
    {
        var (transport, handler) = CreateTransport();
        handler.ResponseBody = "{\"result\":\"OK\"}";
        transport.SetHttpAccessPassword("1234");

        await transport.SendJsonAsync<SuccessResponse>(HttpMethod.Get, "busybar/version");

        Assert.Equal("1234", handler.LastRequest!.Headers.GetValues("x-api-token").Single());
    }

    [Fact]
    public async Task SendJsonAsync_ThrowsBusyBarApiException_OnNon2xxResponse()
    {
        var (transport, handler) = CreateTransport();
        handler.ResponseStatusCode = System.Net.HttpStatusCode.BadRequest;
        handler.ResponseBody = "{\"error\":\"Invalid parameter\",\"code\":400}";

        var exception = await Assert.ThrowsAsync<BusyBarApiException>(
            () => transport.SendJsonAsync<SuccessResponse>(HttpMethod.Get, "busybar/version"));

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("Invalid parameter", exception.ErrorBody!.Error);
    }

    [Fact]
    public async Task SendJsonAsync_ThrowsTimeoutException_WhenRequestExceedsTimeout()
    {
        var (transport, handler) = CreateTransport(TimeSpan.FromMilliseconds(50));
        handler.ResponseDelay = TimeSpan.FromSeconds(2);

        await Assert.ThrowsAsync<TimeoutException>(
            () => transport.SendJsonAsync<SuccessResponse>(HttpMethod.Get, "busybar/version"));
    }

    [Fact]
    public async Task SendJsonAsync_ThrowsOperationCanceledException_WhenCallerCancels()
    {
        var (transport, handler) = CreateTransport();
        handler.ResponseDelay = TimeSpan.FromSeconds(2);
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(20));

        await Assert.ThrowsAsync<TaskCanceledException>(
            () => transport.SendJsonAsync<SuccessResponse>(HttpMethod.Get, "busybar/version", cancellationToken: cts.Token));
    }

    [Fact]
    public async Task SendJsonAsync_ThrowsOperationCanceledException_WhenCallerCancels_AndRequestOptionsSet()
    {
        var (transport, handler) = CreateTransport();
        handler.ResponseDelay = TimeSpan.FromSeconds(2);
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(20));
        var options = new RequestOptions { Timeout = TimeSpan.FromSeconds(3) };

        await Assert.ThrowsAsync<TaskCanceledException>(
            () => transport.SendJsonAsync<SuccessResponse>(
                HttpMethod.Get, "busybar/version", options: options, cancellationToken: cts.Token));
    }

    [Fact]
    public async Task SendBinaryUploadAsync_SendsOctetStreamContentType()
    {
        var (transport, handler) = CreateTransport();
        handler.ResponseBody = "{\"result\":\"OK\"}";
        using var body = new MemoryStream(new byte[] { 1, 2, 3 });

        await transport.SendBinaryUploadAsync<SuccessResponse>(
            HttpMethod.Post, "busybar/assets/upload",
            query: new Dictionary<string, string?> { ["application_name"] = "app", ["file"] = "data.png" },
            requestBody: body);

        Assert.Equal("application/octet-stream", handler.LastRequest!.Content!.Headers.ContentType!.MediaType);
    }

    [Fact]
    public async Task SendBinaryDownloadAsync_ReturnsRawResponseStream()
    {
        var (transport, handler) = CreateTransport();
        handler.ResponseBody = "raw-bytes";
        handler.ResponseContentType = "application/octet-stream";

        await using var stream = await transport.SendBinaryDownloadAsync(HttpMethod.Get, "busybar/storage/read");
        using var reader = new StreamReader(stream);
        Assert.Equal("raw-bytes", await reader.ReadToEndAsync());
    }

    [Fact]
    public async Task SendJsonAsync_ThrowsTimeoutException_WhenBodyStallsAfterFastHeaders()
    {
        // Headers arrive immediately; only the body content is delayed past the configured timeout. Before the
        // fix, SendCoreAsync used HttpCompletionOption.ResponseHeadersRead and the timeout/linked cancellation
        // tokens were disposed the moment headers came back, so a stalled body was never bounded by this timeout.
        var (transport, handler) = CreateTransport(TimeSpan.FromMilliseconds(100));
        handler.BodyDelay = TimeSpan.FromSeconds(5);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        await Assert.ThrowsAsync<TimeoutException>(
            () => transport.SendJsonAsync<SuccessResponse>(HttpMethod.Get, "busybar/version"));
        sw.Stop();

        Assert.True(sw.Elapsed < TimeSpan.FromSeconds(2), $"Expected timeout well under the 5s body stall, took {sw.Elapsed}.");
    }

    [Fact]
    public async Task SendJsonAsync_ThrowsOperationCanceledException_WhenRequestOptionsCancelDuringBodyRead()
    {
        // The default timeout is generous (well past the assertion window) so only the RequestOptions
        // cancellation token — not the timeout — should be able to trip this. Before the fix, RequestOptions
        // .CancellationToken never reached the body-read phase at all, so this cancellation would be ignored
        // once headers had already been received.
        var (transport, handler) = CreateTransport(TimeSpan.FromSeconds(5));
        handler.BodyDelay = TimeSpan.FromSeconds(5);
        using var optionsCts = new CancellationTokenSource();
        optionsCts.CancelAfter(TimeSpan.FromMilliseconds(100));
        var options = new RequestOptions { CancellationToken = optionsCts.Token };

        var sw = System.Diagnostics.Stopwatch.StartNew();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => transport.SendJsonAsync<SuccessResponse>(HttpMethod.Get, "busybar/version", options: options));
        sw.Stop();

        Assert.True(sw.Elapsed < TimeSpan.FromSeconds(2), $"Expected cancellation well under the 5s body stall, took {sw.Elapsed}.");
    }

    [Fact]
    public async Task TimeoutException_Message_DoesNotContainQueryStringValue()
    {
        var (transport, handler) = CreateTransport(TimeSpan.FromMilliseconds(50));
        handler.ResponseDelay = TimeSpan.FromSeconds(2);
        const string secretKey = "839217";

        var exception = await Assert.ThrowsAsync<TimeoutException>(
            () => transport.SendJsonAsync<SuccessResponse>(
                HttpMethod.Post, "busybar/access",
                query: new Dictionary<string, string?> { ["mode"] = "key", ["key"] = secretKey }));

        Assert.DoesNotContain(secretKey, exception.Message);
        Assert.Contains("access", exception.Message);
    }

    [Fact]
    public async Task SendJsonAsync_IsCloudTrue_PreservesBusybarPrefix()
    {
        var (transport, handler) = CreateTransport(isCloud: true);
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await transport.SendJsonAsync<SuccessResponse>(HttpMethod.Get, "busybar/version");

        Assert.EndsWith("busybar/version", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task SendJsonAsync_IsCloudFalse_StripsBusybarPrefix()
    {
        var (transport, handler) = CreateTransport(isCloud: false);
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await transport.SendJsonAsync<SuccessResponse>(HttpMethod.Get, "busybar/version");

        var requestUri = handler.LastRequest!.RequestUri!.ToString();
        Assert.EndsWith("version", requestUri);
        Assert.DoesNotContain("busybar", requestUri);
    }
}
