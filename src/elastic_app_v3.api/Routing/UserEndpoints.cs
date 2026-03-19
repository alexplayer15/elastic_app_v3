using System.Security.Claims;
using Asp.Versioning;
using Asp.Versioning.Builder;
using elastic_app_v3.api.Errors;
using elastic_app_v3.application.DTOs;
using elastic_app_v3.application.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;

namespace elastic_app_v3.api.Routing
{
    public static class UserEndpoints
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

            elasticAppApi.MapPost(RoutingConstants.UserSignUpEndpoint, async Task<IResult> (
                [FromBody] SignUpRequest request,
                [FromServices] IUserService userService,
                CancellationToken cancellationToken) =>
            {
                var result = await userService.SignUpAsync(request, cancellationToken);

                return result.ToApiResponse(RoutingConstants.UserSignUpEndpoint);
            })
            .WithName(RoutingConstants.UserSignUpEndpointOpenApiName)
            .MapToApiVersion(1);

            elasticAppApi.MapPost(RoutingConstants.UserLoginEndpoint, async Task<IResult> (
                [FromBody] LoginRequest request,
                [FromServices] IUserService userService) =>
            {
                var result = await userService.LoginAsync(request);

                return result.ToApiResponse(RoutingConstants.UserLoginEndpoint);
            })
            .WithName(RoutingConstants.UserLoginEndpointOpenApiName)
            .MapToApiVersion(1);

            elasticAppApi.MapGet(RoutingConstants.GetUserByIdEndpoint, async Task<IResult> (
                ClaimsPrincipal user,
                [FromServices] IUserService userService,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(userIdClaim, out var userId))
                    return TypedResults.Unauthorized();

                var result = await userService.GetUserByIdAsync(userId, cancellationToken);

                return result.ToApiResponse(RoutingConstants.GetUserByIdEndpoint);
            })
            .RequireAuthorization()
            .WithName(RoutingConstants.GetUserByIdEndpointOpenApiName)
            .MapToApiVersion(1);

            elasticAppApi.MapPost(RoutingConstants.PaymentEndpoint, async Task<IResult> (
                [FromBody] PaymentRequest request,
                [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
                [FromServices] IPaymentService paymentService,
                CancellationToken cancellationToken) => 
            {
                if (string.IsNullOrWhiteSpace(idempotencyKey))
                {
                    return Results.BadRequest("Missing Idempotency-Key header");
                }

                var result = await paymentService.AddPayment(request, idempotencyKey, cancellationToken);

                return result.ToApiResponse(RoutingConstants.PaymentEndpoint);
            })
            .MapToApiVersion(1);

            elasticAppApi
                .MapHealthChecks(RoutingConstants.ShallowHealthCheckEndpoint, new HealthCheckOptions
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
                .WithName(RoutingConstants.ShallowHealthCheckEndpoint);
        }
    }
}
