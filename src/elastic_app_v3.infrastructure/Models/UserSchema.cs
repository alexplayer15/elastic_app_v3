namespace elastic_app_v3.infrastructure.Models
{
    public sealed record UserSchema(
        Guid Id,
        string FirstName,
        string LastName,
        string UserName,
        string PasswordHash,
        DateTime CreatedAt
    );
}
