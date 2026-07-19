using CSharpFunctionalExtensions;
using elastic_app_v3.application.DTOs;
using elastic_app_v3.application.DTOs.SignUp;
using elastic_app_v3.domain.Errors;

namespace elastic_app_v3.application.Services.Identity
{
    public interface IUserService
    {
        Task<UnitResult<UserError>> SignUpAsync(
            SignUpRequest request, 
            CancellationToken cancellationToken
        );
        Task<Result<GetUserResponse, UserError>> GetUserByIdAsync(
            Guid userId, 
            CancellationToken cancellationToken
        );
    }
}
