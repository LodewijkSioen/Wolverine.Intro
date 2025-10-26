using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

// Wolverine does't like numbers in the namespace
// ReSharper disable once CheckNamespace
namespace Wolverine.Intro.Api.Middleware;

public static class Configuration
{
    public static void AddHeaderMiddleware(this WolverineHttpOptions options)
    {
        options.AddMiddleware<HeaderMiddleware>(chain => chain.HasAttribute<HeaderMiddlewareAttribute>());
    }
}

public class HeaderMiddlewareAttribute : Attribute;
public class HeaderMiddleware
{
    public record MiddlewareResult(string? Value);

    public MiddlewareResult Before([FromHeader(Name = "x-middleware")] string? headerValue)
    {
        return new(headerValue);
    }
}

public class HeaderMiddlewareEndpoint
{
    public record BeforeResult(string Value);

    public record Result(string? Handler, string? Before, string? Middleware);

    public BeforeResult Before([FromHeader(Name = "x-before")] string valueBefore)
    {
        return new (valueBefore);
    }

    [HeaderMiddleware]
    [WolverineGet("/middleware/headers")]
    public Result HandleGet([FromHeader(Name = "x-handler")] string valueEndpoint, BeforeResult before, HeaderMiddleware.MiddlewareResult middleware)
    {
        return new(valueEndpoint, before.Value, middleware.Value);
    }
}