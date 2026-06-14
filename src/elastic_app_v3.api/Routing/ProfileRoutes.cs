using System.Security.Claims;
using CSharpFunctionalExtensions;
using elastic_app_v3.api.Routing.Constants;
using elastic_app_v3.application.Commands;
using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.application.Services.Profiles;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace elastic_app_v3.api.Routing;
public static class ProfileRoutes
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPatch(EndpointConstants.UpdateProfileEndpoint, async Task<IResult> (
            ClaimsPrincipal user,
            [FromBody] UpdateProfileRequest request,
            CancellationToken cancellationToken,
            [FromServices] IMediator mediator) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(userIdClaim, out var userId))
                    return TypedResults.Unauthorized();
                
                var languages = request.Languages is null 
                    ? Maybe<IReadOnlyList<LanguageModel>>.None 
                    : Maybe<IReadOnlyList<LanguageModel>>.From([..request.Languages.Select(l => new LanguageModel(l.Type, l.Proficiency))]);

                var command = new UpdateProfileCommand(
                    request.Bio,
                    languages,
                    request.Hobbies,
                    userId); //okay to have this logic in routing?

                var result = await mediator.Send(command, cancellationToken);

                return result.ToApiResponse(EndpointConstants.UpdateProfileEndpoint);
            })
            .RequireAuthorization()
            .MapToApiVersion(1);
        
        group.MapGet(EndpointConstants.GetProfilePictureUrls, IResult (
                ClaimsPrincipal user,
                [FromServices] IProfileService profileService) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(userIdClaim, out var userId))
                    return TypedResults.Unauthorized();

                var result = profileService.GetProfilePictureUrls(userId);
                return result.ToApiResponse(EndpointConstants.GetProfilePictureUrls);
            })
            .RequireAuthorization()
            .MapToApiVersion(1);
        
        group.MapPatch(EndpointConstants.SaveProfilePicture, async Task<IResult> (
                ClaimsPrincipal user,
                [FromBody] SaveProfilePictureRequest request,
                [FromServices] IProfileService profileService,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(userIdClaim, out var userId))
                    return TypedResults.Unauthorized();

                var result = await profileService.SaveProfilePicture(userId, request,  cancellationToken);
                return result.ToApiResponse(EndpointConstants.SaveProfilePicture);
            })
            .RequireAuthorization()
            .MapToApiVersion(1);
    }
}
