using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.application.Mapping;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Models;
using FluentResults;
using FluentResults.Extensions;

namespace elastic_app_v3.application.Services.Profiles;
public class ProfileService(IProfileRepository profileRepository) : IProfileService
{
    private readonly IProfileRepository _profileRepository = profileRepository;
    public async Task<Result<UpdateProfileResponse>> UpdateProfile(
        ProfileUpdate update, 
        CancellationToken cancellationToken)
    {
        return await _profileRepository.UpdateProfile(update, cancellationToken)
            .Map(updatedProfile => updatedProfile.ToDto());
    }
}
