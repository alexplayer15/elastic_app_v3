using elastic_app_v3.Domain;
using elastic_app_v3.DTOs;
using elastic_app_v3.Errors;

namespace elastic_app_v3.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users = [];
        public Result<SignUpResponse> TryAdd(User user)
        {
            var userExists = CheckIfUserNameExists(user.GetUserName());

            if (userExists)
            {
                var error = UserErrors.UserAlreadyExistsError(user.GetUserName());
                return Result<SignUpResponse>.Failure(error);
            }

            _users.Add(user);

            var signUpResponse = new SignUpResponse(user.GetId());
            return Result<SignUpResponse>.Success(signUpResponse);
        }
        private bool CheckIfUserNameExists(string userName)
        {
            return _users.Any(u => u.UserName == userName);
        }
    }
}
