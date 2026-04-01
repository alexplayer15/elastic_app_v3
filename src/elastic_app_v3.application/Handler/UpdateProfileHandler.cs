using elastic_app_v3.application.Commands;
using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.application.Mapping;
using elastic_app_v3.application.Services.Profiles;
using FluentResults;
using MediatR;

namespace elastic_app_v3.application.Handler;
public class UpdateProfileHandler(IProfileService profileService) : IRequestHandler<UpdateProfileCommand, Result<UpdateProfileResponse>>
{
    private readonly IProfileService _profileService = profileService;
    public async Task<Result<UpdateProfileResponse>> Handle(
        UpdateProfileCommand request, 
        CancellationToken cancellationToken)
    {
        var profileUpdate = request.ToDomainModel();
        return await _profileService.UpdateProfile(profileUpdate, cancellationToken);
    }
}
