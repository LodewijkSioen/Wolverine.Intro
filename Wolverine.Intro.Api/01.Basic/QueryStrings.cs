using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

// Wolverine does't like numbers in the namespace
// ReSharper disable once CheckNamespace
namespace Wolverine.Intro.Api.Basic;

public class QueryStrings
{
    [WolverineGet("/querystring/simple/raw")]
    public string Simple(string code)
    {
        return code;
    }

    [WolverineGet("/querystring/simple/attr")]
    public string SimpleAttr([FromQuery] string code)
    {
        return code;
    }

    [WolverineGet("/querystring/simple/attrNamed")]
    public string SimpleNamed([FromQuery(Name = "c")] string code)
    {
        return code;
    }

    [WolverineGet("/querystring/collection/raw")]
    public string[] Collection(string[] codes)
    {
        return codes;
    }

    /*
     DOES NOT COMPILE
     Is not the Wolverine way of doing stuff
    [WolverineGet("/querystring/collection/attr")]
    public string[] CollectionAttr([FromQuery]string[] codes)
    {
        return codes;
    }

    [WolverineGet("/querystring/collection/attrNamed")]
    public string[] CollectionAttrNamed([FromQuery(Name = "c")] string[] codes)
    {
        return codes;
    }
    */

    public record ComplexType([FromQuery]string Code, [FromQuery] string[] Codes);
    [WolverineGet("/querystring/complex/attr")]
    public object ComplexAttr([FromQuery]ComplexType complex)
    {
        return new { complex.Code, complex.Codes };
    }

    public record ComplexTypeWithAttribute([FromQuery(Name = "c")] string Code, [FromQuery(Name = "cs")]string[] Codes);
    [WolverineGet("/querystring/complex/attrNamed")]
    public object ComplexNamed([FromQuery]ComplexTypeWithAttribute complex)
    {
        return new { complex.Code, complex.Codes };
    }
}