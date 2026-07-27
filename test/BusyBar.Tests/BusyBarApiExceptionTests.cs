using System.Net;
using Busy.Bar;
using Xunit;

namespace BusyBar.Tests;

public class BusyBarApiExceptionTests
{
    [Fact]
    public void Message_IncludesStatusCodeAndErrorText_WhenErrorBodyParsed()
    {
        var errorBody = new BusyBarErrorBody("Invalid parameter", 400);
        var ctor = typeof(BusyBarApiException).GetConstructor(
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null, new[] { typeof(HttpStatusCode), typeof(string), typeof(string), typeof(BusyBarErrorBody) }, null)!;
        var exception = (BusyBarApiException)ctor.Invoke(new object?[]
        {
            HttpStatusCode.BadRequest, "Bad Request", "{\"error\":\"Invalid parameter\",\"code\":400}", errorBody
        });

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal(errorBody, exception.ErrorBody);
        Assert.Contains("400", exception.Message);
        Assert.Contains("Invalid parameter", exception.Message);
    }
}
