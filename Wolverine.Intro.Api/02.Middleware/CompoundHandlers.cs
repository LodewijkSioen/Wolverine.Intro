using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
// Wolverine does't like numbers in the namespace
// ReSharper disable once CheckNamespace
namespace Wolverine.Intro.Api.Middleware;

/// <summary>
/// https://wolverinefx.net/guide/handlers/#compound-handlers
/// </summary>
public class CompoundHandlerEndpoint
{
    public record LoadedEntity(int Id);

    public LoadedEntity? Load(int entityId)
    {
        return entityId > 1
            ? new(entityId)
            : null;
    }

    public ProblemDetails Validate(LoadedEntity entity)
    {
        return entity.Id > 5
            ? new() { Status = 400, Title = "Id is too high" }
            : WolverineContinue.NoProblems;
    }

    [WolverineGet("/middleware/compound/{entityId}")]
    public IResult Get([Required] LoadedEntity entity)
    {
        return Results.Ok(entity);
    }    
}

/// <summary>
/// While the possible names for before/after/finally are restricted, the order
/// of operations within one lifecycle element is determined by the dependencies
/// of the method parameters.
/// </summary>
public class CompoundHandlerOrderEndpoint
{
    public record LoadedEntity(int Id);

    public record SomethingElse;

    public SomethingElse Before(LoadedEntity entity)
    {
        return new();
    }

    public ProblemDetails Load(LoadedEntity entity, SomethingElse something)
    {
        return WolverineContinue.NoProblems;
    }

    public LoadedEntity Validate(int entityId)
    {
        return new(entityId);
    }

    [WolverineGet("/middleware/order/{entityId}")]
    public IResult Get(LoadedEntity entity, SomethingElse something)
    {
        return Results.Ok();
    }
}