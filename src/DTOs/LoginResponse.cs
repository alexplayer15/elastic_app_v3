namespace elastic_app_v3.DTOs
{
    public sealed record LoginResponse(
        string AccessToken,
        string RefreshToken,
        string TokenType,
        int? ExpiresInMinutes
    );
}
