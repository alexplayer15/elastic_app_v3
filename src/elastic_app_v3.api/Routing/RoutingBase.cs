using Asp.Versioning;
using Asp.Versioning.Builder;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

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
               .AddFluentValidationAutoValidation()
               .WithApiVersionSet(apiVersionSet);

            IdentityRoutes.Map(elasticAppApi);
            PaymentRoutes.Map(elasticAppApi);
            ProfileRoutes.Map(elasticAppApi);
            HealthCheckRoutes.Map(elasticAppApi);
        }
    }
}
