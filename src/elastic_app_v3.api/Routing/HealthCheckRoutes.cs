using elastic_app_v3.api.Routing.Constants;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace elastic_app_v3.api.Routing;
public static class HealthCheckRoutes
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapHealthChecks(EndpointConstants.ShallowHealthCheckEndpoint, new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    status = report.Status.ToString()
                });
            }
        })
        .HasApiVersion(1.0)
        .WithName(OpenApiConstants.ShallowHealthCheckOpenApiName);
    }
}
