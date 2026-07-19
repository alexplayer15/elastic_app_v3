namespace elastic_app_v3.domain.Errors.Profile;

public class BioTooLongError : ProfileError
{
    public override string Message { get; } = "Bio cannot exceed 500 characters.";
}