using elastic_app_v3.application.Commands;
using elastic_app_v3.application.DTOs.Profile;
using CSharpFunctionalExtensions;
using elastic_app_v3.domain.Errors;

namespace elastic_app_v3.application.Services.Profiles;
public interface IProfileService
{
    Task<Result<UpdateProfileResponse, ProfileError>> UpdateProfile(
        UpdateProfileCommand update,
        CancellationToken cancellationToken
    );
    Result<GetProfilePictureUrlResponse, ProfileError> GetProfilePictureUrls(Guid userId);

    Task<UnitResult<ProfileError>> SaveProfilePicture(
        Guid userId,
        SaveProfilePictureRequest request,
        CancellationToken cancellationToken
    );
}
