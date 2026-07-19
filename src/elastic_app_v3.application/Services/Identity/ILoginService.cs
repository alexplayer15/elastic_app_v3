using elastic_app_v3.application.DTOs.Login;
using CSharpFunctionalExtensions;
using elastic_app_v3.domain.Errors;

namespace elastic_app_v3.application.Services.Identity;
public interface ILoginService
{
    Task<Result<TokenDto, UserError>> LoginAsync(
        LoginRequest request, 
        CancellationToken cancellationToken
    );
}
