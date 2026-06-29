using elastic_app_v3.domain.Entities;
using FluentResults;

namespace elastic_app_v3.domain.Abstractions;
public interface IProfileRepository
{
    Task<Result<Profile>> GetProfileByUserId(Guid userId, CancellationToken cancellationToken);
        
    Task<Result<Profile>> UpdateProfile(
        Profile profile,
        CancellationToken cancellationToken
    );
    
    Task<Result> SaveProfilePicture(
        Guid userId,
        string objectUrl,
        CancellationToken cancellationToken
    );
}
