namespace Wolverine.Intro.Tests._04.Validation;

public class ValidationTests
{
    [Test]
    public async Task HappyPath()
    {
        var response = await SystemUnderTest.Host.Scenario(s =>
        {
            s.Post
                .Json(new { Value = "NewName" })
                .ToUrl("/validation/fluent");
            s.StatusCodeShouldBe(200);
        });

        var result = await response.ReadAsTextAsync();
        await VerifyJson(result);
    }

    [Test]
    public async Task EmptyName_IsInvalid()
    {
        var response = await SystemUnderTest.Host.Scenario(s =>
        {
            s.Post
                .Json(new { Value = "" })
                .ToUrl("/validation/fluent");
            s.StatusCodeShouldBe(400);
        });

        var result = await response.ReadAsTextAsync();
        await VerifyJson(result);
    }

    [Test]
    public async Task NewName_MustBeDifferent()
    {
        var response = await SystemUnderTest.Host.Scenario(s =>
        {
            s.Post
                .Json(new { Value = "CurrentName" })
                .ToUrl("/validation/fluent");
            s.StatusCodeShouldBe(400);
        });

        var result = await response.ReadAsTextAsync();
        await VerifyJson(result);
    }

    [Test]
    public async Task EmptyName_IsInvalid_ForDataAnnotations()
    {
        var response = await SystemUnderTest.Host.Scenario(s =>
        {
            s.Post
                .Json(new { Value = "" })
                .ToUrl("/validation/dataannotations");
            s.StatusCodeShouldBe(400);
        });

        var result = await response.ReadAsTextAsync();
        await VerifyJson(result);
    }
}