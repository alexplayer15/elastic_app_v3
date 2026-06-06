using FluentResults;

namespace elastic_app_v3.domain.Abstractions;

public interface IProfilePictureDataStore
{
    Result<string> GetProfilePictureUrl(Guid userId);
}