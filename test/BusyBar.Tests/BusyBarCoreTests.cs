using Xunit;

namespace BusyBar.Tests;

public class BusyBarCoreTests
{
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
