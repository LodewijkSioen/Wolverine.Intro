using JasperFx.CodeGeneration;
using JasperFx.CodeGeneration.Frames;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Wolverine.Http;
using Wolverine.Http.CodeGen;


// Wolverine does't like numbers in the namespace
// ReSharper disable once CheckNamespace
namespace Wolverine.Intro.Api.FluentValidation;

public class DataAnnotationsValidationPolicy : IHttpPolicy
{
    public void Apply(IReadOnlyList<HttpChain> chains, GenerationRules rules, JasperFx.IServiceContainer container)
    {
        foreach (var chain in chains.Where(x => x.HasRequestType))
        {
            Apply(chain, container);
        }
    }

    public void Apply(HttpChain chain, JasperFx.IServiceContainer container)
    {
        chain.Metadata.ProducesValidationProblem();

        var method =
            typeof(DataAnnotationsValidationExecutor).GetMethod(nameof(DataAnnotationsValidationExecutor.Validate))!
                .MakeGenericMethod(chain.RequestType!);

        var methodCall = new MethodCall(typeof(DataAnnotationsValidationExecutor), method)
        {
            CommentText = "Execute FluentValidation validators"
        };

        var maybeResult = new MaybeEndWithResultFrame(methodCall.ReturnVariable!);
        chain.Middleware.InsertRange(0, [methodCall, maybeResult]);
    }
}


public static class DataAnnotationsValidationExecutor
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IResult Validate<T>(T message, IServiceProvider services)
    {
        // First, validate the incoming request of type T
        var context = new ValidationContext(message, services, null);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(message, context, results);


        // If there are any errors, create a ProblemDetails result and return
        // that to write out the validation errors and otherwise stop processing
        if (!isValid)
        {
            var state = results
                .SelectMany(x => x.MemberNames)
                .Distinct()
                .ToDictionary(
                    x => x,
                    x => results
                        .Where(r => r.MemberNames.Any(n => n == x))
                        .Select(r => r.ErrorMessage ?? "unknown error")
                        .ToArray());

            var details = new ValidationProblemDetails(state);
            return Results.Problem(details);
        }

        // Everything is good, full steam ahead!
        return WolverineContinue.Result();
    }
}