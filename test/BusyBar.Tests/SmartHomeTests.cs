using Busy.Bar;
using BusyBar.Tests.Internal;
using Xunit;

namespace BusyBar.Tests;

public class SmartHomeTests
{
    private static (Busy.Bar.BusyBar bar, FakeHttpMessageHandler handler) CreateClient()
    {
        var handler = new FakeHttpMessageHandler();
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        return (new Busy.Bar.BusyBar(http, new BusyBarOptions()), handler);
    }

    [Fact]
    public async Task SmartHomePairingGetAsync_ParsesNestedLatestPairingStatus()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"fabric_count\":1,\"latest_pairing_status\":{\"value\":\"completed_successfully\",\"timestamp\":1769436711}}";

        var info = await bar.SmartHomePairingGetAsync();

        Assert.Equal(1, info.FabricCount);
        Assert.Equal(PairingStatusValue.CompletedSuccessfully, info.LatestPairingStatus!.Value);
    }

    [Fact]
    public async Task SmartHomePairingStartAsync_ParsesQrAndManualCode()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"available_until\":\"1769437579000\",\"qr_code\":\"MT:XYZ\",\"manual_code\":\"1155-360-0377\"}";

        var payload = await bar.SmartHomePairingStartAsync();

        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("1155-360-0377", payload.ManualCode);
    }

    [Fact]
    public async Task SmartHomeSwitchSetAsync_SerializesStateAndStartup()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.SmartHomeSwitchSetAsync(new SmartHomeSwitchState(true, SwitchStartup.Toggle));

        Assert.Contains("\"state\":true", handler.LastRequestBody);
        Assert.Contains("\"startup\":\"toggle\"", handler.LastRequestBody);
    }
}
