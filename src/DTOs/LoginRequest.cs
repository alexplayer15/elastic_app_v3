namespace elastic_app_v3.DTOs
{
    public sealed record LoginRequest(
        string UserName,
        string Password
    );
}
