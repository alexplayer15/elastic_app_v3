namespace elastic_app_v3.application.DTOs.Login
{
    public sealed record LoginResponse(
        string AccessToken,
        string RefreshToken,
        string TokenType,
        int? ExpiresInMinutes
    );
}
