using elastic_app_v3.application.DTOs;
using elastic_app_v3.application.DTOs.SignUp;
using FluentResults;

namespace elastic_app_v3.application.Services.Identity
{
    public interface IUserService
    {
        Task<Result> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken);
        Task<Result<GetUserResponse>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
