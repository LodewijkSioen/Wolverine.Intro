using Alba;
using JasperFx.Core;
using Moq;
using Wolverine.Intro.Api.Outbox;

namespace Wolverine.Intro.Tests._03.Outbox;

public class OutboxTests
{
    [Test]
    public async Task OutboxEvent_Success()
    {
        var (session, _) = await SystemUnderTest.Host.TrackedScenario(s =>
        {
            s.Post
                .Json(new { Test = "1" })
                .ToUrl("/Cascading/outbox");
            s.StatusCodeShouldBeOk();
        });

        Assert.That(session.MessageSucceeded.SingleMessage<OutboxEvent>(),
            Is.Not.Null);
    }

    [Test]
    public async Task OutboxEvent_Transactional_Success()
    {
        var (session, _) = await SystemUnderTest.Host.TrackedScenario(s =>
        {
            s.Post
                .Json(new { Test = "1" })
                .ToUrl("/Cascading/outbox/transactional");
            s.StatusCodeShouldBe(202);
        });

        Assert.That(session.MessageSucceeded.SingleMessage<OutboxEvent>(),
            Is.Not.Null);
    }

    [Test]
    public async Task OutboxEvent_Failure_MovedToDeadLetter_AfterRetry()
    {
        MockSomeExternalDependency.Mock.Setup(m => m.DoStuffAsync()).ThrowsAsync(new());

        var (session, _) = await SystemUnderTest.Host.TrackedScenario(s =>
        {
            s.Post
                .Json(new { Test = "1" })
                .ToUrl("/Cascading/outbox");
            s.StatusCodeShouldBeOk();
        });

        Assert.That(session.AllExceptions(), Has.Count.EqualTo(1));
        Assert.That(session.ExecutionStarted.MessagesOf<OutboxEvent>().ToList(),
            Has.Count.EqualTo(4));
        Assert.That(session.MovedToErrorQueue.SingleMessage<OutboxEvent>(),
            Is.Not.Null);
    }

    [Test]
    public async Task OutboxEvent_Success_AfterRetry()
    {
        MockSomeExternalDependency.Mock.SetupSequence(m => m.DoStuffAsync())
            .ThrowsAsync(new())
            .ThrowsAsync(new())
            .Returns(Task.CompletedTask);

        var (session, _) = await SystemUnderTest.Host.TrackedScenario(s =>
        {
            s.Post
                .Json(new { Test = "1" })
                .ToUrl("/Cascading/outbox");
            s.StatusCodeShouldBeOk();
        });

        Assert.That(session.ExecutionStarted.MessagesOf<OutboxEvent>().ToList(),
            Has.Count.EqualTo(3));
        Assert.That(session.MessageSucceeded.SingleMessage<OutboxEvent>(),
            Is.Not.Null);
    }

    [Test]
    public async Task OutboxEvent_Scheduled_Transactional()
    {
        var (session, _) = await SystemUnderTest.Host.TrackedScenario(s =>
        {
            s.Post
                .Json(new { Test = "1" })
                .ToUrl("/Cascading/scheduled");
            s.StatusCodeShouldBe(204);
        });

        session.ExecutionFinished.SingleMessage<OutboxEvent>();
        session.MessageSucceeded.SingleMessage<OutboxEvent>();
        session.Scheduled.SingleMessage<OutboxEventScheduled>();

        var scheduledSession = await session.PlayScheduledMessagesAsync(5.Seconds());

        scheduledSession.ExecutionFinished.SingleMessage<OutboxEventScheduled>();
    }

    [Test]
    public async Task OutboxEvent_Scheduled()
    {
        var (session, _) = await SystemUnderTest.Host.TrackedScenario(s =>
        {
            s.Post
                .Json(new { Test = "1" })
                .ToUrl("/Cascading/scheduled/nodbcontext");
            s.StatusCodeShouldBe(204);
        });

        session.ExecutionFinished.SingleMessage<OutboxEvent>();
        session.MessageSucceeded.SingleMessage<OutboxEvent>();
        session.Scheduled.SingleMessage<OutboxEventScheduled>();

        var scheduledSession = await session.PlayScheduledMessagesAsync(5.Seconds());

        scheduledSession.ExecutionFinished.SingleMessage<OutboxEventScheduled>();
    }

    [TearDown]
    public void TearDown()
    {
        MockSomeExternalDependency.Mock.Reset();
    }
}