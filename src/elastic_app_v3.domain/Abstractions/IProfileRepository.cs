using CSharpFunctionalExtensions;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.domain.Errors;

namespace elastic_app_v3.domain.Abstractions;
public interface IProfileRepository
{
    Task<Result<Profile, ProfileError>> GetProfileByUserId(
        Guid userId, 
        CancellationToken cancellationToken
    );
        
    Task<Result<Profile, ProfileError>> UpdateProfile(
        Profile profile,
        CancellationToken cancellationToken
    );
    
    Task<UnitResult<ProfileError>> SaveProfilePicture(
        Guid userId,
        string objectUrl,
        CancellationToken cancellationToken
    );
}
