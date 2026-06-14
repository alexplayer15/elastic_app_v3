using CSharpFunctionalExtensions;
using elastic_app_v3.application.Commands;
using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.application.Mapping;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.ValueObjects;
using FluentResults.Extensions;
using Result = FluentResults.Result;

namespace elastic_app_v3.application.Services.Profiles;
public class ProfileService(
    IProfileRepository profileRepository,
    IProfilePictureDataStore profilePictureDataStore
) : IProfileService
{
    private readonly IProfileRepository _profileRepository = profileRepository;
    private readonly IProfilePictureDataStore _profilePictureDataStore =  profilePictureDataStore;
    public async Task<FluentResults.Result<UpdateProfileResponse>> UpdateProfile(
        UpdateProfileCommand update, 
        CancellationToken cancellationToken)
    {
        var profileResult = await _profileRepository.GetProfileByUserId(update.UserId, cancellationToken);

        if (profileResult.IsFailed)
        {
            return Result.Fail<UpdateProfileResponse>(profileResult.Errors);
        }

        var profile = profileResult.Value;
        var updatesResult = ApplyIfPresent(update.Bio, profile.UpdateBio)
            .Bind(() => ApplyIfPresent(update.Languages, languages =>
                profile.UpdateLanguages([..languages.Select(l => new Language(l.Type, l.Proficiency))])))
            .Bind(() => ApplyIfPresent(update.Hobbies, profile.UpdateHobbies));
        
        if (updatesResult.IsFailed)
            return Result.Fail<UpdateProfileResponse>(updatesResult.Errors);
        
        return await _profileRepository.UpdateProfile(profile, cancellationToken)
            .Map(updatedProfile => updatedProfile.ToDto());
    }
    public FluentResults.Result<GetProfilePictureUrlResponse> GetProfilePictureUrls(Guid userId)
    {
        return _profilePictureDataStore.GetProfilePictureUrls(userId)
            .Map(urls => new GetProfilePictureUrlResponse(urls.PreSignedUrl, urls.ObjectUrl));
    }
    public async Task<Result> SaveProfilePicture(
        Guid userId, 
        SaveProfilePictureRequest request, 
        CancellationToken cancellationToken
    )
    {
        return await _profileRepository.SaveProfilePicture(userId, request.ObjectUrl, cancellationToken);
    }
    
    private static Result ApplyIfPresent<T>(Maybe<T> maybe, Func<T, Result> update)
    {
        return maybe.HasValue ? update(maybe.Value) : Result.Ok();
    }
}
