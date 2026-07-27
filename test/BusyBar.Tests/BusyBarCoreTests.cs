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
