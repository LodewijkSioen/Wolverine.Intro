using Alba;
using Wolverine.Intro.Api.Middleware;

namespace Wolverine.Intro.Tests._02.Middleware;

public class HeaderMiddlewareTests
{
    [Test]
    public async Task Get_HappyPath()
    {
        var result = await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/middleware/headers");
            s.WithRequestHeader("x-handler", "handler");
            s.WithRequestHeader("x-before", "before");
            s.WithRequestHeader("x-middleware", "middleware");
            s.StatusCodeShouldBeOk();
        });

        var response = await result.ReadAsJsonAsync<HeaderMiddlewareEndpoint.Result>();

        Assert.That(response.Handler, Is.EqualTo("handler"));
        Assert.That(response.Before, Is.EqualTo("before"));
        Assert.That(response.Middleware, Is.EqualTo("middleware"));
    }
}