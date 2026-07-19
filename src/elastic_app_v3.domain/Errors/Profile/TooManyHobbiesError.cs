namespace elastic_app_v3.domain.Errors.Profile;

public class TooManyHobbiesError : ProfileError
{
    public override string Message { get; } = "You cannot have more than 10 hobbies. No-one has that much time.";
}