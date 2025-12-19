using elastic_app_v3.DTOs;
using elastic_app_v3.Services;
using elastic_app_v3.Enums;
using Microsoft.AspNetCore.Mvc;
using elastic_app_v3.Domain;

namespace elastic_app_v3.Routing
{
    public static class UserEndpoints
    {
        public static void Map(WebApplication app)
        {
            app.MapPost(RoutingConstants.UserSignUpEndpoint, async Task<IResult> (
                [FromBody] SignUpRequest request,
                [FromServices] IUserService userService) =>
            {
                var result = await userService.SignUpAsync(request);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value)
                    : result.Error.ErrorCategory switch
                    {
                        ErrorCategory.UserAlreadyExists => TypedResults.Conflict(result.Error),
                        ErrorCategory.SqlTimeoutException => TypedResults.StatusCode(504),
                        _ => TypedResults.StatusCode(500)
                    };
            })
            .WithName(RoutingConstants.UserSignUpEndpointOpenApiName)
            .WithOpenApi();

            app.MapGet(RoutingConstants.GetUserByIdEndpoint, async Task<IResult> (
                Guid userId,
                [FromServices] IUserService userService) =>
            {
                var result = await userService.GetUserByIdAsync(userId);

                return result.IsSuccess
                    ? TypedResults.Ok(result.Value)
                    : TypedResults.NotFound(result.Error);
            })
            .WithName(RoutingConstants.GetUserByIdEndpointOpenApiName)
            .WithOpenApi();
        }
    }
}
