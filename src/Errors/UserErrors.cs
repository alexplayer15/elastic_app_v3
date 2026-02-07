using elastic_app_v3.Enums;

namespace elastic_app_v3.Errors
{
    public static class UserErrors
    {
        public static Error UserAlreadyExistsError(string userName)
        {
            return new Error("User.AlreadyExists", ErrorCategory.UserAlreadyExists, $"User with username: {userName} already exists" );
        }

        public static Error UserDoesNotExistError = new("User.NotExists", ErrorCategory.UserDoesNotExist, "User does not exist");

        public static Error UserLoginCredentialsDoesNotExistError =
            new("User.LoginCredentialsNotExists", ErrorCategory.UserLoginCredentialsDoesNotExist, "A user with those credentials does not exists");
    }
}
