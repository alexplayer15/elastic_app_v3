using System.Security.Claims;
using elastic_app_v3.api.Routing.Constants;
using elastic_app_v3.application.Services.Identity;
using elastic_app_v3.application.DTOs.Login;
using Microsoft.AspNetCore.Mvc;
using elastic_app_v3.application.DTOs.SignUp;
using FluentResults.Extensions;
using Microsoft.AspNetCore.CookiePolicy;

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
            HttpContext httpContext,
            [FromBody] LoginRequest request,
            [FromServices] ILoginService loginService,
            CancellationToken cancellationToken) =>
        {
            var result = await loginService.LoginAsync(request, cancellationToken)
                .Tap(loginResponse => AddAccessTokenToHttpContext(
                    httpContext, 
                    loginResponse.AccessToken, 
                    loginResponse.ExpiresInMinutes
                ));
                
            return result
                .ToResult()
                .ToApiResponse(EndpointConstants.UserLoginEndpoint);
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

    private static void AddAccessTokenToHttpContext(
        HttpContext httpContext,
        string accessToken,
        int expiresInMinutes
    )
    {
        httpContext.Response.Cookies.Append("accessToken", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(expiresInMinutes)
        });
    }
    
}
