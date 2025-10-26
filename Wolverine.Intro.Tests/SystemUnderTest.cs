using Alba;
using JasperFx.CommandLine;
using JasperFx.Core;
using JasperFx.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Respawn;
using Wolverine.Intro.Api;
using Wolverine.Intro.Api.Outbox;
using Wolverine.Persistence.Durability;
using Wolverine.Tracking;

namespace Wolverine.Intro.Tests;

[SetUpFixture]
public class SystemUnderTest
{
    private static Respawner _snapshot = null!;
    public static IAlbaHost Host { get; private set; } = null!;
    public static string ConnectionString =
        "Server=.;Database=wolverine_intro;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True;";

    [OneTimeSetUp]
    public async Task Init()
    {
        JasperFxEnvironment.AutoStartHost = true;
        //var options = new DbContextOptionsBuilder<TestDbContext>()
        //    .UseSqlServer(ConnectionString)
        //    .Options;
        //await using var dbContext = new TestDbContext(options);
        //await dbContext.Database.EnsureDeletedAsync();
        //await dbContext.Database.MigrateAsync();

        _snapshot = await Respawner.CreateAsync(ConnectionString, new()
        {
            SchemasToExclude = ["wolverine"],
            TablesToIgnore = ["__EFMigrationsHistory"]
        });

        Host = await AlbaHost.For<Program>(x =>
        {
            x.ConfigureServices(services =>
            {
                services.RunWolverineInSoloMode();
                services.AddResourceSetupOnStartup();
                services.DisableAllExternalWolverineTransports();

                services.Replace(
                    ServiceDescriptor.Transient<ISomeExternalDependency, MockSomeExternalDependency>());
            });
        });
    }

    public static async Task Reset()
    {
        await Host.ResetResourceState();
        await using var store = Host.Services.GetRequiredService<IMessageStore>();
        await store.Admin.ClearAllAsync();
        await _snapshot.ResetAsync(ConnectionString);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await Host.DisposeAsync();
    }
}

public static class ScenarioExtensions
{
    public static async Task<(ITrackedSession, IScenarioResult)> TrackedScenario(this IAlbaHost host, Action<Scenario> configuration)
    {
        IScenarioResult result = null!;

        // The outer part is tying into Wolverine's test support
        // to "wait" for all detected message activity to complete
        var tracked = await host
            .TrackActivity()
            .DoNotAssertOnExceptionsDetected()
            .ExecuteAndWaitAsync(Action);

        return (tracked, result);

        async Task Action(IMessageContext arg)
        {
            // The inner part here is actually making an HTTP request
            // to the system under test with Alba
            result = await host.Scenario(configuration);
        }
    }
}

public abstract class BaseIntegrationFixture
{
    [SetUp]
    public async Task Setup()
    {
        await SystemUnderTest.Reset();
        MockSomeExternalDependency.Mock.Reset();
    }
}

public class MockSomeExternalDependency : ISomeExternalDependency
{
    public static Mock<ISomeExternalDependency> Mock = new();

    public Task DoStuffAsync()
    {
        return Mock.Object.DoStuffAsync();
    }
}