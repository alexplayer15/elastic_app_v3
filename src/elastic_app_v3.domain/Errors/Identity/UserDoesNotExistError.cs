namespace elastic_app_v3.domain.Errors.Identity;

public sealed class UserDoesNotExistError : UserError
{
    public override string Message { get; } = "User does not exist";
}

