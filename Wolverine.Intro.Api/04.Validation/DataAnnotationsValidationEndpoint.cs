using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;


// Wolverine does't like numbers in the namespace
// ReSharper disable once CheckNamespace
namespace Wolverine.Intro.Api.Validation;

public class DataAnnotationsValidationEndpoint
{
    public record Request([property: Required]string Value);

    public record Entity(string Name);

    public Entity Load()
    {
        return new("CurrentName");
    }

    // This is Business Validation
    public ProblemDetails Validate(Request request, Entity entity, HttpContext httpContext)
    {
        return entity.Name == request.Value
            ? new ValidationProblemDetails(new Dictionary<string, string[]>{
                    { "Value", ["Cannot change to the same name"] }})
                { Status = 400 }
            : WolverineContinue.NoProblems;
    }

    [WolverinePost("validation/dataannotations")]
    public IResult Post(Request message, Entity entity)
    {
        return Results.Ok($"Changed name from {entity.Name} to {message.Value}");
    }
}