using elastic_app_v3.application.DTOs.Login;
using FluentResults;

namespace elastic_app_v3.application.Services.Identity;
public interface ILoginService
{
    Task<Result<TokenData>> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}
