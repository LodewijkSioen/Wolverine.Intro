using Wolverine.Http;

// Wolverine does't like numbers in the namespace
// ReSharper disable once CheckNamespace
namespace Wolverine.Intro.Api.Basic;

public class SimpleHandler
{
    public record Request(int A, int B);

    public record Response(int Sum);

    public Response Handle(Request request)
    {
        var sum = request.A + request.B;
        return new(sum);
    }
}

public class SimpleEndpoints
{
    [WolverineGet("/basic/{number}")]
    public IResult HandleGet(int number)
    {
        return number < 5
            ? Results.BadRequest("Number is less than 5")
            : Results.Ok();
    }

    
    public record AddCommand(int First, int Second);
    public record AddResponse(int Sum);
    [WolverinePost("/basic/add")]
    public AddResponse HandleAddCommand(AddCommand command)
    {
        return new(command.First + command.Second);
    }
}