namespace elastic_app_v3.domain.Errors.Identity;

public sealed class UserAlreadyExistsError : UserError
{
    public override string Message { get; } = "User already exists";
}
