using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.domain.Models;
using FluentResults;

namespace elastic_app_v3.application.Services.Profiles;
public interface IProfileService
{
    Task<Result<UpdateProfileResponse>> UpdateProfile(
        ProfileUpdate update,
        CancellationToken cancellationToken
    );
}
