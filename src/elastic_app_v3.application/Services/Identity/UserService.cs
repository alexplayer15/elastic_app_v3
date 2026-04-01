using elastic_app_v3.domain.Abstractions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.application.DTOs;
using FluentResults;
using elastic_app_v3.application.DTOs.SignUp;

namespace elastic_app_v3.application.Services.Identity
{
    public class UserService(
        IUserRepository userDbRepository, 
        IPasswordHasher<User> passwordHasher
    ) : IUserService
    {
        private readonly IUserRepository _userDbRepository = userDbRepository;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
        public async Task<Result> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName
            };

            var hashedPassword = _passwordHasher.HashPassword(user, request.Password);

            user.SetPasswordHash(hashedPassword);

            return await _userDbRepository.AddAsync(user, cancellationToken);
        }
        public async Task<Result<GetUserResponse>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var userResult = await _userDbRepository.GetUserByIdAsync(userId, cancellationToken);

            return userResult
                .Map(user => new GetUserResponse(user.FirstName, user.LastName, user.UserName));
        }
    }
}
