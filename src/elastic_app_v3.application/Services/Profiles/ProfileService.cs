using CSharpFunctionalExtensions;
using elastic_app_v3.application.Commands;
using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.application.Mapping;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Errors;
using elastic_app_v3.domain.ValueObjects;

namespace elastic_app_v3.application.Services.Profiles;
public class ProfileService(
    IProfileRepository profileRepository,
    IProfilePictureDataStore profilePictureDataStore
) : IProfileService
{
    private readonly IProfileRepository _profileRepository = profileRepository;
    private readonly IProfilePictureDataStore _profilePictureDataStore =  profilePictureDataStore;
    public async Task<Result<UpdateProfileResponse, ProfileError>> UpdateProfile(
        UpdateProfileCommand update, 
        CancellationToken cancellationToken)
    {
        var profileResult = await _profileRepository.GetProfileByUserId(update.UserId, cancellationToken);

        if (profileResult.IsFailure)
        {
            return Result.Failure<UpdateProfileResponse, ProfileError>(profileResult.Error);
        }

        var profile = profileResult.Value;
        var updatesResult = ApplyIfPresent(update.Bio, profile.UpdateBio)
            .Bind(() => ApplyIfPresent(update.Languages, languages =>
                profile.UpdateLanguages([..languages.Select(l => new Language(l.Type, l.Proficiency))])))
            .Bind(() => ApplyIfPresent(update.Hobbies, profile.UpdateHobbies));

        if (updatesResult.IsFailure)
        {
            return Result.Failure<UpdateProfileResponse, ProfileError>(updatesResult.Error);
        }
        
        return await _profileRepository.UpdateProfile(profile, cancellationToken)
            .Map(updatedProfile => updatedProfile.ToDto());
    }
    public Result<GetProfilePictureUrlResponse, ProfileError> GetProfilePictureUrls(Guid userId)
    {
        return _profilePictureDataStore.GetProfilePictureUrls(userId)
            .Map(urls => new GetProfilePictureUrlResponse(urls.PreSignedUrl, urls.ObjectUrl));
    }
    public async Task<UnitResult<ProfileError>> SaveProfilePicture(Guid userId,
        SaveProfilePictureRequest request,
        CancellationToken cancellationToken)
    {
        return await _profileRepository.SaveProfilePicture(userId, request.ObjectUrl, cancellationToken);
    }
    
    private static UnitResult<ProfileError>ApplyIfPresent<T>(Maybe<T> maybe, Func<T, UnitResult<ProfileError>> update)
    {
        return maybe.HasValue ? update(maybe.Value) : UnitResult.Success<ProfileError>();
    }
}
