using Wolverine.Http;

// Wolverine does't like numbers in the namespace
// ReSharper disable once CheckNamespace
namespace Wolverine.Intro.Api.Basic;

public class CascadingEndpoint
{
    public record Response1;
    public record Response2;
    public record Request(string Test);
    [WolverinePost("/cascading")]
    public (IResult, OutgoingMessages) Post(Request request, ILogger logger)
    {
        logger.LogInformation("Request Handled");

        return (Results.Ok("Ok"), [new Response1(), new Response2()]);
    }

}

public class ResponseHandler
{
    public void Handle(CascadingEndpoint.Response1 response, ILogger<ResponseHandler> logger)
    {
        logger.LogInformation("Response 1 Handled");
    }

    public void Handle(CascadingEndpoint.Response2 response, ILogger<ResponseHandler> logger)
    {
        logger.LogInformation("Response 2 Handled");
    }
}