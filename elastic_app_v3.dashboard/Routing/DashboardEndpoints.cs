using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;

namespace elastic_app_v3.dashboard.Routing;
public static class DashboardEndpoints
{
    private const string ElasticAppDashboardBaseRoute = "/elastic-app-dashboard/v{version:apiVersion}";
    public static void Map(WebApplication app)
    {
        ApiVersionSet apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1))
                .ReportApiVersions()
                .Build();

        var elasticAppDashboardApi = app
           .MapGroup(ElasticAppDashboardBaseRoute)
           .WithApiVersionSet(apiVersionSet);

        elasticAppDashboardApi.MapPost("/dashboard", async Task<IResult> (
            [FromBody] Message message,
            CancellationToken cancellationToken) =>
        {
            Console.WriteLine(message.DetailType);

            return Results.Ok(message);
        })
        .MapToApiVersion(1);
    }
}