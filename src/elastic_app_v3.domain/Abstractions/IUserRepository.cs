using CSharpFunctionalExtensions;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.domain.Errors;

namespace elastic_app_v3.domain.Abstractions
{
    public interface IUserRepository
    {
        Task<UnitResult<UserError>> AddAsync(
            User user, 
            CancellationToken cancellationToken
        );
        Task<Result<User, UserError>> GetUserByUsernameAsync(
            string userName,
            CancellationToken cancellationToken
        );
        Task<Result<User, UserError>> GetUserByIdAsync(
            Guid userId, 
            CancellationToken cancellationToken
        );
    }
}
