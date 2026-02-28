using elastic_app_v3.application.DTOs;
using FluentResults;

namespace elastic_app_v3.application.Services
{
    public interface IUserService
    {
        Task<Result<SignUpResponse>> SignUpAsync(SignUpRequest request);
        Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
        Task<Result<GetUserResponse>> GetUserByIdAsync(Guid userId);
    }
}
