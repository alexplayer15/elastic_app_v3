using elastic_app_v3.Domain;
using elastic_app_v3.Errors;
using elastic_app_v3.DTOs;

namespace elastic_app_v3.Repositories
{
    public interface IUserRepository
    {
        Task<Result<Guid>> AddAsync(User user);
        Task<Result<UserSchema>> GetUserByUsernameAsync(LoginRequest loginRequest);
        Task<Result<UserSchema>> GetUserByIdAsync(Guid userId);
    }
}
