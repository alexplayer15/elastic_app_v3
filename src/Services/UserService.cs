using elastic_app_v3.Domain;
using elastic_app_v3.DTOs;
using elastic_app_v3.Repositories;
using elastic_app_v3.Errors;

namespace elastic_app_v3.Services
{
    public class UserService(IUserRepository userDbRepository) : IUserService
    {
        private readonly IUserRepository _userDbRepository = userDbRepository;
        public async Task<Result<SignUpResponse>> SignUpAsync(SignUpRequest request)
        {
            var user = new User(
                request.FirstName,
                request.LastName,
                request.UserName,
                request.Password
            );

            var idResult = await _userDbRepository.AddAsync(user);

            return idResult.Map(id => new SignUpResponse(id));
        }
        public async Task<Result<GetUserResponse>> GetUserByIdAsync(Guid userId)
        {
            var userResult = await _userDbRepository.GetUserByIdAsync(userId);

            return userResult
                .Map(user => new GetUserResponse(user.FirstName, user.LastName, user.UserName, user.CreatedAt));
        }
    }
}
