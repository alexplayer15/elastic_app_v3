using elastic_app_v3.DTOs;
using elastic_app_v3.Services;
using elastic_app_v3.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
                        ErrorCategory.ValidationError => TypedResults.BadRequest(result.Error),
                        ErrorCategory.UserAlreadyExists => TypedResults.Conflict(result.Error),
                        ErrorCategory.SqlTimeoutException => Results.Json(result.Error, statusCode: 504),
                        ErrorCategory.SqlException => Results.Json(result.Error, statusCode: 500),
                        _ => TypedResults.StatusCode(500)
                    };
            })
            .WithName(RoutingConstants.UserSignUpEndpointOpenApiName);

            app.MapPost(RoutingConstants.UserLoginEndpoint, async Task<IResult> (
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
            .WithName(RoutingConstants.UserLoginEndpointOpenApiName);

            app.MapGet(RoutingConstants.GetUserByIdEndpoint, async Task<IResult> (
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
            .WithName(RoutingConstants.GetUserByIdEndpointOpenApiName);
        }
    }
}
