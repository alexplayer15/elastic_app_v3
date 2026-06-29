using elastic_app_v3.application.Commands;
using elastic_app_v3.application.DTOs.Profile;
using FluentResults;

namespace elastic_app_v3.application.Services.Profiles;
public interface IProfileService
{
    Task<Result<UpdateProfileResponse>> UpdateProfile(
        UpdateProfileCommand update,
        CancellationToken cancellationToken
    );
    Result<GetProfilePictureUrlResponse> GetProfilePictureUrls(Guid userId);

    Task<Result> SaveProfilePicture(
        Guid userId,
        SaveProfilePictureRequest request,
        CancellationToken cancellationToken
    );
}
