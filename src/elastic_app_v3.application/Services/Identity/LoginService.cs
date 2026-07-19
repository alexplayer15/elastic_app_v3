using elastic_app_v3.application.DTOs.Login;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Entities;
using CSharpFunctionalExtensions;
using elastic_app_v3.domain.Errors;
using elastic_app_v3.domain.Errors.Identity;
using Microsoft.AspNetCore.Identity;

namespace elastic_app_v3.application.Services.Identity;
public class LoginService(
    IUserRepository userDbRepository,
    ITokenGenerator tokenGenerator,
    IPasswordHasher<User> passwordHasher
    ) : ILoginService
{
    private readonly IUserRepository _userDbRepository = userDbRepository;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    public async Task<Result<TokenDto, UserError>> LoginAsync(
        LoginRequest request, 
        CancellationToken cancellationToken
    )
    {
        return await _userDbRepository.GetUserByUsernameAsync(request.UserName, cancellationToken)
            .Bind(user => VerifyUserPassword(user, user.PasswordHash, request.Password))
            .Bind(user => _tokenGenerator.Generate(user))
            .Map(tokens => new TokenDto(tokens.AccessToken, tokens.RefreshToken, "Bearer", tokens.ExpiresInMinutes));
    }
    private Result<User, UserError> VerifyUserPassword(User user, string userPassword, string requestedPassword)
    {
        var verifiedHashResult = _passwordHasher.VerifyHashedPassword(user, userPassword, requestedPassword);

        return verifiedHashResult == PasswordVerificationResult.Failed ?
            Result.Failure<User, UserError>(new IncorrectPasswordError()) :
            Result.Success<User, UserError>(user);
    }
}
