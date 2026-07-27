using Busy.Bar;
using Busy.Bar.Internal;
using BusyBar.Tests.Internal;
using Xunit;

namespace BusyBar.Tests.Internal;

public class BusyBarTransportTests
{
    private static (BusyBarTransport transport, FakeHttpMessageHandler handler) CreateTransport(TimeSpan? defaultTimeout = null)
    {
        var handler = new FakeHttpMessageHandler();
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        return (new BusyBarTransport(http, defaultTimeout ?? TimeSpan.FromSeconds(3)), handler);
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
        Assert.EndsWith("busybar/name?value=50", handler.LastRequest!.RequestUri!.ToString());
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
}
