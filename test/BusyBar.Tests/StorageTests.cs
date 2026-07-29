using Busy.Bar;
using BusyBar.Tests.Internal;
using Xunit;

namespace BusyBar.Tests;

public class StorageTests
{
    private static (Busy.Bar.BusyBar bar, FakeHttpMessageHandler handler) CreateClient()
    {
        var handler = new FakeHttpMessageHandler();
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://10.0.4.20/") };
        return (new Busy.Bar.BusyBar(http, new BusyBarOptions()), handler);
    }

    [Fact]
    public async Task StorageWriteAsync_SendsBinaryBodyWithPathQuery()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";
        using var content = new MemoryStream(new byte[] { 1 });

        await bar.StorageWriteAsync(new StorageWriteParams("/ext/test.png"), content);

        Assert.Contains("path=%2Fext%2Ftest.png", handler.LastRequest!.RequestUri!.Query);
        Assert.Equal("application/octet-stream", handler.LastRequest.Content!.Headers.ContentType!.MediaType);
    }

    [Fact]
    public async Task StorageReadAsync_ReturnsRawStream()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "file-bytes";

        await using var stream = await bar.StorageReadAsync(new StorageReadParams("/ext/test.png"));
        using var reader = new StreamReader(stream);

        Assert.Equal("file-bytes", await reader.ReadToEndAsync());
    }

    [Fact]
    public async Task StorageListAsync_ParsesMixedFileAndDirElements()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"list\":[{\"type\":\"file\",\"name\":\"test.png\",\"size\":65535},{\"type\":\"dir\",\"name\":\"assets\"}]}";

        var list = await bar.StorageListAsync(new StorageListParams("/ext"));

        Assert.Equal(2, list.List.Count);
        var file = Assert.IsType<StorageFileElement>(list.List[0]);
        Assert.Equal(65535, file.Size);
        Assert.IsType<StorageDirElement>(list.List[1]);
    }

    [Fact]
    public async Task StorageRemoveAsync_SendsDeleteWithPathQuery()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.StorageRemoveAsync(new StorageRemoveParams("/ext/test.png"));

        Assert.Equal(HttpMethod.Delete, handler.LastRequest!.Method);
        Assert.Contains("path=%2Fext%2Ftest.png", handler.LastRequest.RequestUri!.Query);
    }

    [Fact]
    public async Task StorageMkdirAsync_SendsPostWithPathQuery()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.StorageMkdirAsync(new StorageMkdirParams("/ext/newdir"));

        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Contains("path=%2Fext%2Fnewdir", handler.LastRequest.RequestUri!.Query);
    }

    [Fact]
    public async Task StorageRenameAsync_SendsBothPathQueryParams()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"result\":\"OK\"}";

        await bar.StorageRenameAsync(new StorageRenameParams("/ext/a.txt", "/ext/b.txt"));

        Assert.Contains("path=%2Fext%2Fa.txt", handler.LastRequest!.RequestUri!.Query);
        Assert.Contains("new_path=%2Fext%2Fb.txt", handler.LastRequest.RequestUri.Query);
    }

    [Fact]
    public async Task StorageStatusGetAsync_ParsesByteCounts()
    {
        var (bar, handler) = CreateClient();
        handler.ResponseBody = "{\"used_bytes\":123456,\"free_bytes\":654321,\"total_bytes\":777777}";

        var status = await bar.StorageStatusGetAsync();

        Assert.Equal(777777, status.TotalBytes);
    }
}
