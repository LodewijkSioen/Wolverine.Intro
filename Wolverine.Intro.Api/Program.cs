using JasperFx;
using JasperFx.CodeGeneration;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.FluentValidation;
using Wolverine.Http;
using Wolverine.Http.FluentValidation;
using Wolverine.Intro.Api;
using Wolverine.Intro.Api.FluentValidation;
using Wolverine.Intro.Api.Middleware;
using Wolverine.Intro.Api.Outbox;
using Wolverine.SqlServer;

var builder = WebApplication.CreateBuilder(args);
var connectionString = "Server=.;Database=wolverine_intro;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True;";

// Add services to the container.
builder.Host.UseWolverine(opts =>
{
    opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Dynamic;

    opts.Services.AddDbContextWithWolverineIntegration<TestDbContext>(x =>
    {
        x.UseSqlServer(connectionString);
    }, "wolverine");

    opts.UseEntityFrameworkCoreTransactions();

    opts.Policies.AutoApplyTransactions();
    opts.Policies.UseDurableLocalQueues();
    opts.PersistMessagesWithSqlServer(connectionString, "wolverine");

    opts.UseFluentValidation();
});

builder.Services.AddWolverineHttp();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.IncludeXmlComments(Assembly.GetExecutingAssembly());
});

builder.Services.AddTransient<ISomeExternalDependency, SomeExternalDependency>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.MapWolverineEndpoints(opts =>
{
    opts.AddHeaderMiddleware();
    opts.UseFluentValidationProblemDetailMiddleware();
    opts.AddPolicy<DataAnnotationsValidationPolicy>();
    opts.WarmUpRoutes = RouteWarmup.Eager;
});

await app.RunJasperFxCommands(args);
