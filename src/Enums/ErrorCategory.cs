namespace elastic_app_v3.Enums
{
    public enum ErrorCategory
    {
        UserAlreadyExists,
        None,
        UserDoesNotExist,
        SqlException,
        SqlTimeoutException,
        ValidationError,
        UserLoginCredentialsDoesNotExist,
        JwtConfigValidation
    }
}
