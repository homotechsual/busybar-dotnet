using Busy.Bar;
using BusyBar.Tests.Internal;
using Xunit;

namespace BusyBar.Tests;

public class BusyTimerTests
{
    private static (Busy.Bar.BusyBar bar, FakeHttpMessageHandler handler) CreateClient()
    {
        var handler = new FakeHttpMessageHandler();
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        return (new Busy.Bar.BusyBar(http, new BusyBarOptions()), handler);
    }

    [Fact]
    public async Task BusySnapshotGetAsync_ParsesSimpleSnapshot()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = """
        {"snapshot":{"type":"SIMPLE","card_id":"00000000-0000-0000-0000-000000000000","time_left_ms":9000,"is_paused":false,"busy_bar_settings":{"theme":"on_air","show_work_phase_only":false,"trigger_smart_home":true}},"snapshot_timestamp_ms":1761582532251}
        """;

        var snapshot = await bar.BusySnapshotGetAsync();

        var simple = Assert.IsType<BusySnapshotSimple>(snapshot.Snapshot);
        Assert.Equal(9000, simple.TimeLeftMs);
        Assert.False(simple.IsPaused);
        Assert.Equal("on_air", simple.BusyBarSettings!.Theme);
    }

    [Fact]
    public async Task BusySnapshotSetAsync_SerializesIntervalSnapshotWithDiscriminator()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";
        var snapshot = new BusySnapshot(
            new BusySnapshotInterval
            {
                CardId = "00000000-0000-0000-0000-000000000000",
                CurrentInterval = 1,
                CurrentIntervalTimeTotalMs = 60000,
                CurrentIntervalTimeLeftMs = 42690,
                IsPaused = false,
                IntervalSettings = new BusyTimerIntervalSettings
                {
                    IntervalWorkMs = 120000, IntervalRestMs = 60000,
                    IntervalWorkCyclesCount = 3, IsAutostartEnabled = false
                }
            },
            1761582532251);

        await bar.BusySnapshotSetAsync(snapshot);

        Assert.Equal(HttpMethod.Put, handler.LastRequest!.Method);
        Assert.Contains("\"type\":\"INTERVAL\"", handler.LastRequestBody);
        Assert.Contains("\"current_interval\":1", handler.LastRequestBody);
    }

    [Fact]
    public async Task BusyProfileGetAsync_BuildsPathWithSlot()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = """
        {"sort_order":-1,"title":"study","id":"00000000-0000-0000-0000-000000000000","timer_settings":{"type":"SIMPLE","total_time_ms":300000},"busy_bar_settings":{"theme":"on_air","show_work_phase_only":false,"trigger_smart_home":true},"profile_timestamp_ms":1761582532251}
        """;

        var profile = await bar.BusyProfileGetAsync(BusyProfileSlot.Custom);

        Assert.EndsWith("busy/profiles/custom", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("study", profile.Title);
        var simple = Assert.IsType<BusyTimerSimpleSettings>(profile.TimerSettings);
        Assert.Equal(300000, simple.TotalTimeMs);
    }

    [Fact]
    public async Task BusyProfileSetAsync_BuildsPathWithSlotAndSerializesProfile()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";
        var profile = new BusyProfile
        {
            SortOrder = -1,
            Title = "study",
            Id = "00000000-0000-0000-0000-000000000000",
            TimerSettings = new BusyTimerSimpleSettings { TotalTimeMs = 300000 },
            BusyBarSettings = new BusyBarSettings("on_air", false, true),
            ProfileTimestampMs = 1761582532251
        };

        await bar.BusyProfileSetAsync(BusyProfileSlot.Busy, profile);

        Assert.Equal(HttpMethod.Put, handler.LastRequest!.Method);
        Assert.EndsWith("busy/profiles/busy", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("\"title\":\"study\"", handler.LastRequestBody);
    }
}
