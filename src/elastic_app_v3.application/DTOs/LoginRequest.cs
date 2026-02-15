namespace elastic_app_v3.application.DTOs
{
    public sealed record LoginRequest(
        string UserName,
        string Password
    );
}
