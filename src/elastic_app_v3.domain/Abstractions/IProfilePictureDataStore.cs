using elastic_app_v3.domain.DTOs;
using CSharpFunctionalExtensions;
using elastic_app_v3.domain.Errors;

namespace elastic_app_v3.domain.Abstractions;

public interface IProfilePictureDataStore
{
    Result<ProfilePictureUrls, ProfileError> GetProfilePictureUrls(Guid userId);
}