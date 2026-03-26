using System.Security.Claims;
using elastic_app_v3.api.Errors;
using elastic_app_v3.api.Routing.Constants;
using elastic_app_v3.application.Services.Identity;
using elastic_app_v3.application.DTOs.Login;
using Microsoft.AspNetCore.Mvc;
using elastic_app_v3.application.DTOs.SignUp;

namespace elastic_app_v3.api.Routing;
public static class IdentityRoutes
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost(EndpointConstants.UserSignUpEndpoint, async Task<IResult> (
            [FromBody] SignUpRequest request,
            [FromServices] IUserService userService,
            CancellationToken cancellationToken) =>
        {
            var result = await userService.SignUpAsync(request, cancellationToken);
            return result.ToApiResponse(EndpointConstants.UserSignUpEndpoint);
        })
        .WithName(OpenApiConstants.UserSignUpEndpointOpenApiName)
        .MapToApiVersion(1);

        group.MapPost(EndpointConstants.UserLoginEndpoint, async Task<IResult> (
            [FromBody] LoginRequest request,
            [FromServices] IUserService userService) =>
        {
            var result = await userService.LoginAsync(request);
            return result.ToApiResponse(EndpointConstants.UserLoginEndpoint);
        })
        .WithName(OpenApiConstants.UserLoginEndpointOpenApiName)
        .MapToApiVersion(1);

        group.MapGet(EndpointConstants.GetUserByIdEndpoint, async Task<IResult> (
            ClaimsPrincipal user,
            [FromServices] IUserService userService,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                return TypedResults.Unauthorized();

            var result = await userService.GetUserByIdAsync(userId, cancellationToken);
            return result.ToApiResponse(EndpointConstants.GetUserByIdEndpoint);
        })
        .RequireAuthorization()
        .WithName(OpenApiConstants.GetUserByIdEndpointOpenApiName)
        .MapToApiVersion(1);
    }
}
