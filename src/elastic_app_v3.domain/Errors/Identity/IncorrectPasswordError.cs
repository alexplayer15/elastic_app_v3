namespace elastic_app_v3.domain.Errors.Identity;

public sealed class IncorrectPasswordError() : UserError
{
    public override string Message { get; } = "Incorrect password";
}

