using System.Security.Claims;
using elastic_app_v3.api.Routing.Constants;
using elastic_app_v3.application.Commands;
using elastic_app_v3.application.DTOs.Profile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

                var command = new UpdateProfileCommand(
                    request.Bio,
                    request.Languages is null ? null : [.. request.Languages.Select(l => new LanguageModel(l.Type, l.Proficiency))],
                    userId); //okay to have this logic in the routing?

                var result = await mediator.Send(command, cancellationToken);

                return result.ToApiResponse(EndpointConstants.UpdateProfileEndpoint);
            })
            .RequireAuthorization()
            .MapToApiVersion(1);
    }
}
