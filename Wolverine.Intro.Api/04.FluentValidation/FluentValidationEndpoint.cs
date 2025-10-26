using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

// Wolverine does't like numbers in the namespace
// ReSharper disable once CheckNamespace
namespace Wolverine.Intro.Api.FluentValidation;

public class FluentValidationEndpoint
{
    public record Request(string Value);

    public record Entity(string Name);

    //This is input validation
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Value).NotEmpty();
        }
    }

    public Entity Load()
    {
        return new("CurrentName");
    }

    // This is Business Validation
    public ProblemDetails Validate(Request request, Entity entity, HttpContext httpContext)
    {
        return entity.Name == request.Value
            ? new ValidationProblemDetails(new Dictionary<string, string[]>{
                { "Value", ["Cannot change to the same name"] }}){ Status = 400 }
            : WolverineContinue.NoProblems;
    }

    [WolverinePost("validation/fluent")]
    public IResult Post(Request message, Entity entity)
    {
        return Results.Ok($"Changed name from {entity.Name} to {message.Value}");
    }
}