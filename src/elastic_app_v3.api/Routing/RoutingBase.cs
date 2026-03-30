using Asp.Versioning;
using Asp.Versioning.Builder;

namespace elastic_app_v3.api.Routing
{
    public static class RoutingBase
    {
        private const string ElasticAppBaseRoute = "/elastic-app/v{version:apiVersion}";
        public static void Map(WebApplication app)
        {
            ApiVersionSet apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1))
                .ReportApiVersions()
                .Build();

            var elasticAppApi = app
               .MapGroup(ElasticAppBaseRoute)
               .WithApiVersionSet(apiVersionSet);

            IdentityRoutes.Map(elasticAppApi);
            PaymentRoutes.Map(elasticAppApi);
            ProfileRoutes.Map(elasticAppApi);
            HealthCheckRoutes.Map(elasticAppApi);
        }
    }
}
