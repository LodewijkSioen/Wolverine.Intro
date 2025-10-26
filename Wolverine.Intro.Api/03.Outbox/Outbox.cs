using Microsoft.AspNetCore.Mvc;
using Wolverine.Attributes;
using Wolverine.Http;

// Wolverine does't like numbers in the namespace
// ReSharper disable once CheckNamespace
namespace Wolverine.Intro.Api.Outbox;


public record OutboxEvent;

public record OutboxEventScheduled;

public class OutboxEndpoint
{

    [WolverinePost("/cascading/outbox")]
    public (IResult, OutgoingMessages) Post(ILogger logger)
    {
        logger.LogInformation("Request Handled");

        return (Results.Ok("Ok"), [new OutboxEvent()]);
    }

    [WolverinePost("/cascading/outbox/transactional")]
    public (AcceptResponse, OutgoingMessages) PostTransactional(ILogger logger)
    {
        logger.LogInformation("Request Handled");

        return (new("Ok"), [new OutboxEvent()]);
    }

    [WolverinePost("/cascading/scheduled")]
    public OutgoingMessages PostScheduled([FromServices]TestDbContext dbContext, ILogger logger)
    {
        dbContext.Users.Add(new()
        {
            Name = "test"
        });

        logger.LogInformation("Request Handled");

        return  [
            new OutboxEvent(),
            new OutboxEventScheduled().DelayedFor(TimeSpan.FromDays(1))
        ];
    }

    [WolverinePost("/cascading/scheduled/nodbcontext")]
    public OutgoingMessages PostScheduled(ILogger logger)
    {
        logger.LogInformation("Request Handled");

        return [
            new OutboxEvent(),
            new OutboxEventScheduled().DelayedFor(TimeSpan.FromDays(1))
        ];
    }
}

public class OutboxEventHandler
{
    [RetryNow(typeof(Exception), 100, 100, 100)]
    public async Task Handle(OutboxEvent response, ISomeExternalDependency service, ILogger logger)
    {
        logger.LogInformation("Event handled");
        await service.DoStuffAsync();
    }

    public Task Handle(OutboxEventScheduled response, ILogger logger)
    {
        logger.LogInformation("Scheduled event handled");
        return Task.CompletedTask;
    }
}

public interface ISomeExternalDependency
{
    public Task DoStuffAsync();
}

public class SomeExternalDependency : ISomeExternalDependency
{
    public Task DoStuffAsync()
    {
        return Task.CompletedTask;
    }
}