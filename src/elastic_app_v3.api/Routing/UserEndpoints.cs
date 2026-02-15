using System.Security.Claims;
using Asp.Versioning;
using Asp.Versioning.Builder;
using elastic_app_v3.application.DTOs;
using elastic_app_v3.application.Services;
using elastic_app_v3.domain.Result;
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
                [FromServices] IUserService userService) =>
            {
                var result = await userService.SignUpAsync(request);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value)
                    : result.Error.ErrorCategory switch
                    {
                        ErrorCategory.ValidationError => TypedResults.BadRequest(result.Error),
                        ErrorCategory.UserAlreadyExists => TypedResults.Conflict(result.Error),
                        ErrorCategory.SqlTimeoutException => Results.Json(result.Error, statusCode: 504),
                        ErrorCategory.SqlException => Results.Json(result.Error, statusCode: 500),
                        _ => TypedResults.StatusCode(500)
                    };
            })
            .WithName(RoutingConstants.UserSignUpEndpointOpenApiName)
            .MapToApiVersion(1);

            elasticAppApi.MapPost(RoutingConstants.UserLoginEndpoint, async Task<IResult> (
                [FromBody] LoginRequest request,
                [FromServices] IUserService userService) =>
            {
                var result = await userService.LoginAsync(request);

                return result.IsSuccess
                   ? TypedResults.Ok(result.Value)
                   : result.Error.ErrorCategory switch
                   {
                       ErrorCategory.ValidationError => TypedResults.BadRequest(result.Error),
                       ErrorCategory.JwtConfigValidation => Results.Json(result.Error, statusCode: 500),
                       ErrorCategory.UserDoesNotExist => TypedResults.NotFound(result.Error),
                       ErrorCategory.IncorrectPassword => Results.Json(result.Error, statusCode: 401),
                       ErrorCategory.SqlTimeoutException => Results.Json(result.Error, statusCode: 504),
                       ErrorCategory.SqlException => Results.Json(result.Error, statusCode: 500),
                       _ => TypedResults.StatusCode(500)
                   };
            })
            .WithName(RoutingConstants.UserLoginEndpointOpenApiName)
            .MapToApiVersion(1);

            elasticAppApi.MapGet(RoutingConstants.GetUserByIdEndpoint, async Task<IResult> (
                ClaimsPrincipal user,
                [FromServices] IUserService userService) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(userIdClaim, out var userId))
                    return TypedResults.Unauthorized();

                var result = await userService.GetUserByIdAsync(userId);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value)
                    : TypedResults.NotFound(result.Error);
            })
            .RequireAuthorization()
            .WithName(RoutingConstants.GetUserByIdEndpointOpenApiName)
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
