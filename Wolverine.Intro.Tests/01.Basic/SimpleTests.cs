using Alba;
using Wolverine.Intro.Api.Basic;

namespace Wolverine.Intro.Tests._01.Basic;

public class SimpleTests
{
    [Test]
    public async Task Get_HappyPath()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/basic/6");
            s.StatusCodeShouldBeOk();
        });
    }

    [Test]
    public async Task Get_BadRequest()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/basic/4");
            s.StatusCodeShouldBe(400);
        });
    }

    [Test]
    public async Task Add_HappyPath()
    {
        var response = await SystemUnderTest.Host.Scenario(s =>
        {
            s.Post
                .Json(new
                {
                    First = 1,
                    Second = 2,
                })
                .ToUrl("/basic/add");
        });

        var result = await response.ReadAsJsonAsync<SimpleEndpoints.AddResponse>();
        Assert.That(result.Sum, Is.EqualTo(3));
    }
}