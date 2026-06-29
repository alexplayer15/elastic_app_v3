using elastic_app_v3.domain.DTOs;
using FluentResults;

namespace elastic_app_v3.domain.Abstractions;

public interface IProfilePictureDataStore
{
    Result<ProfilePictureUrls> GetProfilePictureUrls(Guid userId);
}