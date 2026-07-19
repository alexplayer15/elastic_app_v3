namespace elastic_app_v3.domain.Errors.Profile;

public class MissingBioError : ProfileError
{
    public override string Message { get; } = "Bio cannot be empty if it is being updated.";
}
