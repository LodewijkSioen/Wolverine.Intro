using Alba;
using Wolverine.Intro.Api.Basic;

namespace Wolverine.Intro.Tests._01.Basic;

public class CascadingTests
{

    [Test]
    public async Task Cascading()
    {
        var (session, _) = await SystemUnderTest.Host.TrackedScenario(s =>
        {
            s.Post
                .Json(new { Test = "1" })
                .ToUrl("/Cascading");
            s.StatusCodeShouldBeOk();
        });

        Assert.That(session.Executed.MessagesOf<CascadingEndpoint.Response1>().ToList(), Has.Count.EqualTo(1));
        Assert.That(session.Executed.MessagesOf<CascadingEndpoint.Response2>().ToList(), Has.Count.EqualTo(1));
    }
}