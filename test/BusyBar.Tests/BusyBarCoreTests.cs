using BusyBar.Tests.Internal;
using Xunit;

namespace BusyBar.Tests;

public class BusyBarCoreTests
{
    [Fact]
    public void OptionsConstructor_SetsHttpClientTimeoutToInfinite()
    {
        // BusyBarTransport enforces its own per-request timeout; HttpClient's default 100s Timeout must be
        // disabled so it can never fire first and mask the documented TimeoutException.
        using var bar = new Busy.Bar.BusyBar(new Busy.Bar.BusyBarOptions());
        var http = (HttpClient)typeof(Busy.Bar.BusyBar)
            .GetField("_http", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .GetValue(bar)!;

        Assert.Equal(System.Threading.Timeout.InfiniteTimeSpan, http.Timeout);
    }

    [Fact]
    public void HttpClientConstructor_IsPublic()
    {
        // Reflection-based check of the constructor's accessibility modifier. This is independent of
        // InternalsVisibleTo("BusyBar.Tests") (see AssemblyInfo.cs) — merely being able to call the constructor
        // from this assembly would NOT prove it is public, since InternalsVisibleTo would let an `internal`
        // constructor compile and pass identically here.
        var ctor = typeof(Busy.Bar.BusyBar).GetConstructor(new[] { typeof(HttpClient), typeof(Busy.Bar.BusyBarOptions) });

        Assert.NotNull(ctor);
        Assert.True(ctor!.IsPublic);
    }

    [Fact]
    public async Task HttpClientConstructor_IsUsable()
    {
        var handler = new FakeHttpMessageHandler { ResponseBody = "{\"name\":\"My Bar\"}" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };

        // The round-trip call proves the constructor is fully wired up (accessibility is covered separately by
        // HttpClientConstructor_IsPublic above).
        using var bar = new Busy.Bar.BusyBar(httpClient, new Busy.Bar.BusyBarOptions());
        var result = await bar.NameGetAsync();

        Assert.Equal("My Bar", result.Name);
    }

    [Fact]
    public void DefaultConstructor_UsesUsbAddressWithHttpScheme()
    {
        using var bar = new Busy.Bar.BusyBar();
        var http = (HttpClient)typeof(Busy.Bar.BusyBar)
            .GetField("_http", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .GetValue(bar)!;

        Assert.Equal("http://10.0.4.20/api/", http.BaseAddress!.ToString());
    }

    [Fact]
    public void Constructor_UsesHttpsScheme_ForCloudHost()
    {
        using var bar = new Busy.Bar.BusyBar(new Busy.Bar.BusyBarOptions { Addr = "api.busy.app", Token = "t" });
        var http = (HttpClient)typeof(Busy.Bar.BusyBar)
            .GetField("_http", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .GetValue(bar)!;

        Assert.Equal("https://api.busy.app/", http.BaseAddress!.ToString());
    }

    [Fact]
    public void Constructor_PreservesExplicitScheme()
    {
        using var bar = new Busy.Bar.BusyBar(new Busy.Bar.BusyBarOptions { Addr = "https://example.com" });
        var http = (HttpClient)typeof(Busy.Bar.BusyBar)
            .GetField("_http", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .GetValue(bar)!;

        Assert.Equal("https://example.com/api/", http.BaseAddress!.ToString());
    }

    [Fact]
    public async Task CloudHost_PreservesFullBusybarPathInRequest()
    {
        var handler = new FakeHttpMessageHandler { ResponseBody = "{\"api_semver\":\"24.3.0\"}" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.busy.app/") };
        using var bar = new Busy.Bar.BusyBar(httpClient, new Busy.Bar.BusyBarOptions { Token = "t" });

        await bar.SystemVersionGetAsync();

        Assert.Contains("busybar/version", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task SetToken_ReflectedAsBearerAuthOnSubsequentRequest()
    {
        var handler = new FakeHttpMessageHandler { ResponseBody = "{\"api_semver\":\"24.3.0\"}" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        using var bar = new Busy.Bar.BusyBar(httpClient, new Busy.Bar.BusyBarOptions());

        bar.SetToken("runtime-token");
        await bar.SystemVersionGetAsync();

        Assert.Equal("Bearer", handler.LastRequest!.Headers.Authorization!.Scheme);
        Assert.Equal("runtime-token", handler.LastRequest.Headers.Authorization.Parameter);
    }

    [Fact]
    public async Task SetHttpAccessPassword_ReflectedAsApiTokenHeaderOnSubsequentRequest()
    {
        var handler = new FakeHttpMessageHandler { ResponseBody = "{\"api_semver\":\"24.3.0\"}" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        using var bar = new Busy.Bar.BusyBar(httpClient, new Busy.Bar.BusyBarOptions());

        bar.SetHttpAccessPassword("runtime-password");
        await bar.SystemVersionGetAsync();

        Assert.Equal("runtime-password", handler.LastRequest!.Headers.GetValues("x-api-token").Single());
    }

    [Fact]
    public async Task Constructor_AppliesHttpAccessPasswordFromOptions()
    {
        var handler = new FakeHttpMessageHandler { ResponseBody = "{\"api_semver\":\"24.3.0\"}" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        using var bar = new Busy.Bar.BusyBar(httpClient, new Busy.Bar.BusyBarOptions { HttpAccessPassword = "initial-password" });

        await bar.SystemVersionGetAsync();

        Assert.Equal("initial-password", handler.LastRequest!.Headers.GetValues("x-api-token").Single());
    }

    [Fact]
    public void HttpClientConstructor_SetsBaseAddress_WhenNotAlreadySet()
    {
        using var httpClient = new HttpClient();

        using var bar = new Busy.Bar.BusyBar(httpClient, new Busy.Bar.BusyBarOptions());

        Assert.Equal("http://10.0.4.20/api/", httpClient.BaseAddress!.ToString());
    }

    [Fact]
    public async Task LocalHost_StripsBusybarPrefixFromRequest()
    {
        var handler = new FakeHttpMessageHandler { ResponseBody = "{\"api_semver\":\"24.3.0\"}" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/api/") };
        using var bar = new Busy.Bar.BusyBar(httpClient, new Busy.Bar.BusyBarOptions());

        await bar.SystemVersionGetAsync();

        var requestUri = handler.LastRequest!.RequestUri!.ToString();
        Assert.Contains("api/version", requestUri);
        Assert.DoesNotContain("busybar", requestUri);
    }
}
