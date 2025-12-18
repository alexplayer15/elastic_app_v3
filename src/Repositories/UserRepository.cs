using elastic_app_v3.Domain;
using elastic_app_v3.Errors;

namespace elastic_app_v3.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users = [];
        public Result<Guid>Add(User user)
        {
            var userExists = CheckIfUserNameExists(user.GetUserName());

            if (userExists)
            {
                var error = UserErrors.UserAlreadyExistsError(user.GetUserName());
                return Result<Guid>.Failure(error);
            }

            _users.Add(user);

            return Result<Guid>.Success(user.GetId());
        }

        public Result<User> GetUserById(Guid userId)
        {
            var user = _users.FirstOrDefault(u => u.GetId() == userId);

            if (user == null)
            {
                return Result<User>.Failure(UserErrors.UserDoesNotExistError);
            }

            return Result<User>.Success(user);
        }
        private bool CheckIfUserNameExists(string userName)
        {
            return _users.Any(u => u.UserName == userName);
        }
    }
}
