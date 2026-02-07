using elastic_app_v3.DTOs;
using elastic_app_v3.Errors;

namespace elastic_app_v3.Services
{
    public interface IUserService
    {
        Task<Result<SignUpResponse>> SignUpAsync(SignUpRequest request);
        Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
        Task<Result<GetUserResponse>> GetUserByIdAsync(Guid userId);
    }
}
