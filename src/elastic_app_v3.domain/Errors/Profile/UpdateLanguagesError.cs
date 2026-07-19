namespace elastic_app_v3.domain.Errors.Profile;

public class UpdateLanguagesError : ProfileError
{
    public override string Message { get; } = "Languages cannot be empty if they are being updated.";
}