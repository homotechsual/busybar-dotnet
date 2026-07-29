using Busy.Bar;
using BusyBar.Tests.Internal;
using Xunit;

namespace BusyBar.Tests;

public class SettingsTests
{
    private static (Busy.Bar.BusyBar bar, FakeHttpMessageHandler handler) CreateClient()
    {
        var handler = new FakeHttpMessageHandler();
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        return (new Busy.Bar.BusyBar(http, new BusyBarOptions()), handler);
    }

    [Fact]
    public async Task AccessGetAsync_ParsesModeAndKeyValid()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"mode\":\"key\",\"key_valid\":true}";

        var info = await bar.AccessGetAsync();

        Assert.Equal(HttpAccessMode.Key, info.Mode);
        Assert.True(info.KeyValid);
    }

    [Fact]
    public async Task AccessSetAsync_IncludesKeyQueryParam_WhenModeIsKey()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.AccessSetAsync(new AccessSetParams(HttpAccessMode.Key, "12345678"));

        Assert.Contains("mode=key", handler.LastRequest!.RequestUri!.Query);
        Assert.Contains("key=12345678", handler.LastRequest.RequestUri.Query);
    }

    [Theory]
    [InlineData(HttpAccessMode.Disabled, "disabled")]
    [InlineData(HttpAccessMode.Enabled, "enabled")]
    public async Task AccessSetAsync_OmitsKeyQueryParam_ForNonKeyModes(HttpAccessMode mode, string expected)
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.AccessSetAsync(new AccessSetParams(mode));

        Assert.Contains($"mode={expected}", handler.LastRequest!.RequestUri!.Query);
        Assert.DoesNotContain("key=", handler.LastRequest.RequestUri.Query);
    }

    [Fact]
    public async Task AccessSetAsync_ThrowsArgumentOutOfRangeException_ForUndefinedMode()
    {
        var (bar, handler) = CreateClient();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => bar.AccessSetAsync(new AccessSetParams((HttpAccessMode)999)));
    }

    [Fact]
    public async Task NameGetAndSet_RoundTripName()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"name\":\"BUSY bar\"}";
        var getResult = await bar.NameGetAsync();
        Assert.Equal("BUSY bar", getResult.Name);

        handler.ResponseBody = "{\"result\":\"OK\"}";
        await bar.NameSetAsync(new NameInfo("New Name"));
        Assert.Contains("\"name\":\"New Name\"", handler.LastRequestBody);
    }
}
