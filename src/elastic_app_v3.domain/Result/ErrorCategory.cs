namespace elastic_app_v3.domain.Result
{
    public enum ErrorCategory
    {
        UserAlreadyExists,
        None,
        UserDoesNotExist,
        SqlException,
        SqlTimeoutException,
        ValidationError,
        JwtConfigValidation,
        IncorrectPassword   
    }
}
