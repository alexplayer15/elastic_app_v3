using elastic_app_v3.Domain;
using elastic_app_v3.DTOs;
using elastic_app_v3.Repositories;
using elastic_app_v3.Errors;

namespace elastic_app_v3.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        public Result<SignUpResponse> TrySignUp(SignUpRequest request)
        {
            var user = new User(
                request.FirstName,
                request.LastName,
                request.UserName,
                request.Password
            );

            return _userRepository.TryAdd(user);
        }
    }
}
