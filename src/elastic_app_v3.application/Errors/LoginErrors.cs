using elastic_app_v3.domain.Result;

namespace elastic_app_v3.application.Errors
{
    public static class LoginErrors
    {
        public static Error IncorrectPasswordError = new("Login.IncorrectPassword", ErrorCategory.IncorrectPassword, "Incorrect password");
    }
}
