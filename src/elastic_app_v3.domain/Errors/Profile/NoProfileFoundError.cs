namespace elastic_app_v3.domain.Errors.Profile;
public class NoProfileFoundError(Guid userId) : ProfileError
{
    public override string Message { get; } = $"No profile associated with user {userId}";
}