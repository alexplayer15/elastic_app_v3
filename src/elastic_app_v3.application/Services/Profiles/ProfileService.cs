using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.application.Mapping;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Models;
using FluentResults;

namespace elastic_app_v3.application.Services.Profiles;
public class ProfileService(IProfileRepository profileRepository) : IProfileService
{
    private readonly IProfileRepository _profileRepository = profileRepository;
    public async Task<Result<UpdateProfileResponse>> UpdateProfile(
        ProfileUpdate update, 
        CancellationToken cancellationToken)
    {
        var profileResult = await _profileRepository.GetProfileByUserId(update.UserId, cancellationToken);

        var profile = profileResult.Value;
        profile.UpdateBio(update.Bio);
        profile.UpdateLanguages(update.Languages);

        var updatedProfile = await _profileRepository.UpdateProfile(profile, cancellationToken);

        if (updatedProfile.IsFailed)
        {
            return updatedProfile.ToResult<UpdateProfileResponse>();
        }

        return updatedProfile.Value.ToDto(); 
    }
}
