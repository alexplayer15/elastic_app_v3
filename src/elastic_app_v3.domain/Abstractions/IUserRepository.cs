using elastic_app_v3.domain.Entities;
using elastic_app_v3.domain.Result;

namespace elastic_app_v3.domain.Abstractions
{
    public interface IUserRepository
    {
        Task<Result<Guid>> AddAsync(User user);
        Task<Result<User>> GetUserByUsernameAsync(string userName);
        Task<Result<User>> GetUserByIdAsync(Guid userId);
    }
}
