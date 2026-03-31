using System.Security.Claims;
using elastic_app_v3.api.Routing.Constants;
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

                request.UserId = userId; //do not like this

                var result = await mediator.Send(request, cancellationToken);

                return result.ToApiResponse(EndpointConstants.UpdateProfileEndpoint);
            })
            .RequireAuthorization()
            .MapToApiVersion(1);
    }
}
