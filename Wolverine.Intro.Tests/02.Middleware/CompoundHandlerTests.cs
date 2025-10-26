using Alba;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Intro.Api.Middleware;

namespace Wolverine.Intro.Tests._02.Middleware;

public class CompoundHandlerTests
{
    [Test]
    public async Task CompoundHandler_HappyPath()
    {
        var result = await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/middleware/compound/2");
            s.StatusCodeShouldBeOk();
        });

        var response = await result.ReadAsJsonAsync<CompoundHandlerEndpoint.LoadedEntity>();
        Assert.That(response.Id, Is.EqualTo(2));
    }


    [Test]
    public async Task CompoundHandler_DoesNotExist()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/middleware/compound/0");
            s.StatusCodeShouldBe(404);
        });

    }

    [Test]
    public async Task CompoundHandler_IsInvalid()
    {
        var result = await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/middleware/compound/10");
            s.StatusCodeShouldBe(400);
        });

        var response = await result.ReadAsJsonAsync<ProblemDetails>();
        Assert.That(response.Title, Is.EqualTo("Id is too high"));
    }

    [Test]
    public async Task CompoundHandler_NamingDoesNotMatter()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/middleware/order/2");
            s.StatusCodeShouldBeOk();
        });
    }
}