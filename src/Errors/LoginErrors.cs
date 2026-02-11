using elastic_app_v3.Enums;

namespace elastic_app_v3.Errors
{
    public static class LoginErrors
    {
        public static Error IncorrectPasswordError = new("Login.IncorrectPassword", ErrorCategory.IncorrectPassword, "Incorrect password");
    }
}
