using Alba;

namespace Wolverine.Intro.Tests._01.Basic;

public class QueryStringTests
{
    [Test]
    public async Task SimpleRaw()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/querystring/simple/raw?code=test");
            s.StatusCodeShouldBe(200);
            s.ContentShouldBe("test");
        });
    }

    [Test]
    public async Task SimpleAttribute()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/querystring/simple/attr?code=test");
            s.StatusCodeShouldBe(200);
            s.ContentShouldBe("test");
        });
    }

    [Test]
    public async Task SimpleAttributeNamed()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/querystring/simple/attrNamed?c=test");
            s.StatusCodeShouldBe(200);
            s.ContentShouldBe("test");
        });
    }

    [Test]
    public async Task CollectionRaw()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/querystring/collection/raw?codes=test1&codes=test2&codes=test3");
            s.StatusCodeShouldBe(200);
            s.ContentShouldBe("""["test1","test2","test3"]""");
        });
    }

    [Test]
    public async Task ComplexAttribute()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/querystring/complex/attr?code=test&codes=test1&codes=test2&codes=test3");
            s.StatusCodeShouldBe(200);
            s.ContentShouldBe("{\"code\":\"test\",\"codes\":[\"test1\",\"test2\",\"test3\"]}");
        });
    }

    [Test]
    public async Task ComplexAttributeNamed()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/querystring/complex/attrNamed?c=test&cs=test1&cs=test2&cs=test3");
            s.StatusCodeShouldBe(200);
            s.ContentShouldBe("{\"code\":\"test\",\"codes\":[\"test1\",\"test2\",\"test3\"]}");
        });
    }
}