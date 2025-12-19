using elastic_app_v3.Domain;
using elastic_app_v3.Errors;

namespace elastic_app_v3.Repositories
{
    public interface IUserRepository
    {
        Task<Result<Guid>> AddAsync(User user);
        Task<Result<UserSchema>> GetUserByIdAsync(Guid userId);
    }
}
