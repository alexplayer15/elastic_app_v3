using elastic_app_v3.application.Commands;
using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.application.Services.Profiles;
using CSharpFunctionalExtensions;
using elastic_app_v3.domain.Errors;
using MediatR;

namespace elastic_app_v3.application.Handler;
public class UpdateProfileHandler(IProfileService profileService) : IRequestHandler<UpdateProfileCommand, Result<UpdateProfileResponse, ProfileError>>
{
    private readonly IProfileService _profileService = profileService;
    public async Task<Result<UpdateProfileResponse, ProfileError>> Handle(
        UpdateProfileCommand command, 
        CancellationToken cancellationToken
    )
    {
        return await _profileService.UpdateProfile(command, cancellationToken);
    }
}
