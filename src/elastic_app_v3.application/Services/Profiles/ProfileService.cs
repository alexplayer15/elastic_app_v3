using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.application.Mapping;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Models;
using FluentResults;
using FluentResults.Extensions;

namespace elastic_app_v3.application.Services.Profiles;
public class ProfileService(
    IProfileRepository profileRepository,
    IProfilePictureDataStore profilePictureDataStore
) : IProfileService
{
    private readonly IProfileRepository _profileRepository = profileRepository;
    private readonly IProfilePictureDataStore _profilePictureDataStore =  profilePictureDataStore;
    public async Task<Result<UpdateProfileResponse>> UpdateProfile(
        ProfileUpdate update, 
        CancellationToken cancellationToken)
    {
        return await _profileRepository.UpdateProfile(update, cancellationToken)
            .Map(updatedProfile => updatedProfile.ToDto());
    }
    public Result<GetProfilePictureUrlResponse> GetProfilePictureUrls(Guid userId)
    {
        return _profilePictureDataStore.GetProfilePictureUrls(userId)
            .Map(urls => new GetProfilePictureUrlResponse(urls.PreSignedUrl, urls.ObjectUrl));
    }
}
