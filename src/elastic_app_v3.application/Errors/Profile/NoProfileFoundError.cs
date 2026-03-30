using FluentResults;

namespace elastic_app_v3.application.Errors.Profile;
public class NoProfileFoundError(Guid userId) : Error()
{
    public readonly string ErrorDescription = $"No profile associated with user {userId}";
}