using elastic_app_v3.Domain;
using elastic_app_v3.DTOs;
using elastic_app_v3.Repositories;
using elastic_app_v3.Errors;
using FluentValidation;
using elastic_app_v3.Tokens;
using Microsoft.AspNetCore.Identity;

namespace elastic_app_v3.Services
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
        public async Task<Result<SignUpResponse>> SignUpAsync(SignUpRequest request)
        {
            var validationResult = _signUpRequestValidator.Validate(request);

            if (!validationResult.IsValid)
            {
                var errorDescription = string.Join("; ",
                    validationResult.Errors.Select(e => e.ErrorMessage));

                return Result<SignUpResponse>.Failure(ValidationErrors.ValidationError(errorDescription));
            }

            var user = new User(
                request.FirstName,
                request.LastName,
                request.UserName
            );

            var hashedPassword = _passwordHasher.HashPassword(user, request.Password);

            user.SetPasswordHash(hashedPassword);

            var idResult = await _userDbRepository.AddAsync(user);

            return idResult.Map(id => new SignUpResponse(id));
        }
        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var validationResult = _loginRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                var errorDescription = string.Join("; ",
                  validationResult.Errors.Select(e => e.ErrorMessage));

                return Result<LoginResponse>.Failure(ValidationErrors.ValidationError(errorDescription)); // could split validation errors based on endpoint
            }

            var getUserResult = await _userDbRepository.GetUserByUsernameAsync(request);

            if (!getUserResult.IsSuccess)
            {
                return getUserResult
                    .MapError(_ => new LoginResponse(string.Empty, string.Empty, string.Empty, null)); //seems off to map this all with empty values
            }

            var user = new User(
                getUserResult.Value.FirstName,
                getUserResult.Value.LastName,
                getUserResult.Value.UserName
            )
            {
                Id = getUserResult.Value.Id
            };

            var verifiedHashResult = _passwordHasher.VerifyHashedPassword(user, getUserResult.Value.PasswordHash, request.Password);

            if(verifiedHashResult == PasswordVerificationResult.Failed)
            {
                return Result<LoginResponse>.Failure(LoginErrors.IncorrectPasswordError);
            }

            return await _tokenGenerator.Generate(user); //what if this fails?
        }
        public async Task<Result<GetUserResponse>> GetUserByIdAsync(Guid userId)
        {
            var userResult = await _userDbRepository.GetUserByIdAsync(userId);

            return userResult
                .Map(user => new GetUserResponse(user.FirstName, user.LastName, user.UserName, user.CreatedAt));
        }
    }
}
