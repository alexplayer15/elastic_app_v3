using elastic_app_v3.domain.Entities;
using elastic_app_v3.domain.ValueObjects;
using FluentResults;

namespace elastic_app_v3.domain.Abstractions;
public interface IProfileRepository
{
    Task<Result<Profile>> UpdateProfile(
        Profile profile,
        CancellationToken cancellationToken);
    Task<Result<Profile>> GetProfileByUserId(Guid userId, CancellationToken cancellationToken);
}
