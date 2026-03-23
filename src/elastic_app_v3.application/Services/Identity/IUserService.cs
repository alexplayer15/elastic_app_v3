using elastic_app_v3.application.DTOs;
using elastic_app_v3.application.DTOs.Login;
using elastic_app_v3.application.DTOs.SingUp;
using FluentResults;

namespace elastic_app_v3.application.Services.Identity
{
    public interface IUserService
    {
        Task<Result<SignUpResponse>> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken);
        Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
        Task<Result<GetUserResponse>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
