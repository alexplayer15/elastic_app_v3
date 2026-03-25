using elastic_app_v3.domain.Abstractions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.application.DTOs;
using elastic_app_v3.application.Errors;
using FluentResults;
using elastic_app_v3.application.DTOs.Login;
using elastic_app_v3.application.DTOs.SingUp;
using elastic_app_v3.domain.Events;

namespace elastic_app_v3.application.Services.Identity
{
    public class UserService(
        IUserRepository userDbRepository, 
        IValidator<SignUpRequest> signUpRequestValidator,
        IValidator<LoginRequest> loginRequestValidator,
        IPasswordHasher<User> passwordHasher,
        ITokenGenerator tokenGenerator
    ) : IUserService
    {
        private readonly IUserRepository _userDbRepository = userDbRepository;
        private readonly IValidator<SignUpRequest> _signUpRequestValidator = signUpRequestValidator;
        private readonly IValidator<LoginRequest> _loginRequestValidator = loginRequestValidator;
        private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
        public async Task<Result<SignUpResponse>> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _signUpRequestValidator.Validate(request);

            if (!validationResult.IsValid)
            {
                var errorDescription = string.Join("; ",
                    validationResult.Errors.Select(e => e.ErrorMessage));

                return Result.Fail(new ValidationError(errorDescription));
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName
            };

            var hashedPassword = _passwordHasher.HashPassword(user, request.Password);

            user.SetPasswordHash(hashedPassword);

            var idResult = await _userDbRepository.AddAsync(user, cancellationToken);

            return idResult.Map(id => new SignUpResponse(id));
        }
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

            if(verifiedHashResult == PasswordVerificationResult.Failed)
            {
                return Result.Fail(new IncorrectPasswordError());
            }

            var tokenResult = _tokenGenerator.Generate(getUserResult.Value); //what if this fails? 

            return tokenResult
                .Map(tokens => new LoginResponse(tokens.AccessToken, tokens.RefreshToken, "Bearer", tokens.ExpiresInMinutes));
        }
        public async Task<Result<GetUserResponse>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var userResult = await _userDbRepository.GetUserByIdAsync(userId, cancellationToken);

            return userResult
                .Map(user => new GetUserResponse(user.FirstName, user.LastName, user.UserName));
        }
    }
}
