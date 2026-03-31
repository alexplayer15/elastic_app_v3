using elastic_app_v3.application.DTOs.Login;
using elastic_app_v3.application.Errors;
using elastic_app_v3.application.Errors.Identity;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Entities;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace elastic_app_v3.application.Services.Identity;
public class LoginService(
    IValidator<LoginRequest> loginRequestValidator,
    IUserRepository userDbRepository,
    ITokenGenerator tokenGenerator,
    IPasswordHasher<User> passwordHasher
    ) : ILoginService
{
    private readonly IValidator<LoginRequest> _loginRequestValidator = loginRequestValidator;
    private readonly IUserRepository _userDbRepository = userDbRepository;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var validationResult = _loginRequestValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorDescription = string.Join("; ",
              validationResult.Errors.Select(e => e.ErrorMessage));

            return Result.Fail(new ValidationError(errorDescription));
        }

        var getUserResult = await _userDbRepository.GetUserByUsernameAsync(request.UserName);

        if (!getUserResult.IsSuccess)
        {
            return getUserResult.ToResult<LoginResponse>();
        }

        var verifiedHashResult = _passwordHasher.VerifyHashedPassword(getUserResult.Value, getUserResult.Value.PasswordHash, request.Password);

        if (verifiedHashResult == PasswordVerificationResult.Failed)
        {
            return Result.Fail(new IncorrectPasswordError());
        }

        var tokenResult = _tokenGenerator.Generate(getUserResult.Value); //what if this fails? 

        return tokenResult
            .Map(tokens => new LoginResponse(tokens.AccessToken, tokens.RefreshToken, "Bearer", tokens.ExpiresInMinutes));
    }
}
