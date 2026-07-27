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
    public async Task HttpClientConstructor_IsPublicAndUsable()
    {
        var handler = new FakeHttpMessageHandler { ResponseBody = "{\"name\":\"My Bar\"}" };
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };

        // The constructor being callable at all (from this assembly, without relying on InternalsVisibleTo)
        // proves it is public; the round-trip call proves it is fully wired up.
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

        Assert.Equal("http://10.0.4.20/", http.BaseAddress!.ToString());
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

        Assert.Equal("https://example.com/", http.BaseAddress!.ToString());
    }
}
