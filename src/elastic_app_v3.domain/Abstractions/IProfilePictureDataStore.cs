using elastic_app_v3.domain.Models;
using FluentResults;

namespace elastic_app_v3.domain.Abstractions;

public interface IProfilePictureDataStore
{
    Result<ProfilePictureUrls> GetProfilePictureUrls(Guid userId);
}