using elastic_app_v3.Domain;
using elastic_app_v3.DTOs;
using elastic_app_v3.Repositories;
using elastic_app_v3.Errors;

namespace elastic_app_v3.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        public Result<SignUpResponse> SignUp(SignUpRequest request)
        {
            var user = new User(
                request.FirstName,
                request.LastName,
                request.UserName,
                request.Password
            );

            var idResult = _userRepository.Add(user);

            return idResult.Map(id => new SignUpResponse(id));
        }

        public Result<GetUserResponse> GetUserById(Guid userId)
        {
            var userResult = _userRepository.GetUserById(userId);

            return userResult
                .Map(user => new GetUserResponse(user.FirstName, user.LastName, user.UserName));
        }
    }
}
