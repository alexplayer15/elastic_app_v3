using elastic_app_v3.domain.Entities;
using elastic_app_v3.domain.Models;
using FluentResults;

namespace elastic_app_v3.domain.Abstractions;
public interface IProfileRepository
{
    Task<Result<Profile>> UpdateProfile(
        ProfileUpdate profile,
        CancellationToken cancellationToken
    );
    
    Task<Result> SaveProfilePicture(
        Guid userId,
        string objectUrl,
        CancellationToken cancellationToken
    );
}
