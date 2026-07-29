using Busy.Bar;
using BusyBar.Tests.Internal;
using Xunit;

namespace BusyBar.Tests;

public class UpdateTests
{
    private static (Busy.Bar.BusyBar bar, FakeHttpMessageHandler handler) CreateClient()
    {
        var handler = new FakeHttpMessageHandler();
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        return (new Busy.Bar.BusyBar(http, new BusyBarOptions()), handler);
    }

    [Fact]
    public async Task UpdateFirmwareAsync_SendsBinaryTarBody()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";
        using var tar = new MemoryStream(new byte[] { 1, 2, 3 });

        await bar.UpdateFirmwareAsync(tar);

        Assert.EndsWith("update", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("application/octet-stream", handler.LastRequest.Content!.Headers.ContentType!.MediaType);
    }

    [Fact]
    public async Task UpdateCheckAsync_SendsPost()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.UpdateCheckAsync();

        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.EndsWith("update/check", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task UpdateInstallAsync_SendsVersionQueryParam()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.UpdateInstallAsync(new UpdateInstallParams("1.2.3"));

        Assert.Contains("version=1.2.3", handler.LastRequest!.RequestUri!.Query);
    }

    [Fact]
    public async Task UpdateAbortDownloadAsync_SendsPost()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.UpdateAbortDownloadAsync();

        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.EndsWith("update/abort_download", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task UpdateAutoupdateGetAsync_ParsesSettings()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"is_enabled\":true,\"interval_start\":\"02:00\",\"interval_end\":\"04:00\"}";

        var settings = await bar.UpdateAutoupdateGetAsync();

        Assert.True(settings.IsEnabled);
        Assert.Equal("02:00", settings.IntervalStart);
    }

    [Fact]
    public async Task UpdateStatusGetAsync_ParsesNestedInstallAndCheckStatus()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = """
        {"install":{"is_allowed":true,"event":"none","action":"none","status":"ok","detail":"","download":{"speed_bytes_per_sec":0,"received_bytes":0,"total_bytes":0}},"check":{"available_version":"1.2.3","event":"stop","status":"available"}}
        """;

        var status = await bar.UpdateStatusGetAsync();

        Assert.Equal(UpdateInstallResultStatus.Ok, status.Install!.Status);
        Assert.Equal(UpdateCheckResultStatus.Available, status.Check!.Status);
        Assert.Equal("1.2.3", status.Check.AvailableVersion);
    }

    [Fact]
    public async Task UpdateChangelogGetAsync_SendsVersionQueryParam()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"changelog\":\"Bug fixes\"}";

        var result = await bar.UpdateChangelogGetAsync(new UpdateChangelogParams("1.2.3"));

        Assert.Contains("version=1.2.3", handler.LastRequest!.RequestUri!.Query);
        Assert.Equal("Bug fixes", result.Changelog);
    }

    [Fact]
    public async Task UpdateAutoupdateSetAsync_OmitsUnsetFieldsFromJsonBody()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.UpdateAutoupdateSetAsync(new AutoupdateSettings(IsEnabled: true));

        Assert.Contains("\"is_enabled\":true", handler.LastRequestBody);
        Assert.DoesNotContain("interval_start", handler.LastRequestBody);
    }
}
