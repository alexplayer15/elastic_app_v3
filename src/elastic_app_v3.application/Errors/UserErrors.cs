using elastic_app_v3.domain.Result;

namespace elastic_app_v3.application.Errors
{
    public static class UserErrors
    {
        public static Error UserAlreadyExistsError(string userName)
        {
            return new Error("User.AlreadyExists", ErrorCategory.UserAlreadyExists, $"User with username: {userName} already exists" );
        }

        public static Error UserDoesNotExistError = new("User.NotExists", ErrorCategory.UserDoesNotExist, "User does not exist");
    }
}
