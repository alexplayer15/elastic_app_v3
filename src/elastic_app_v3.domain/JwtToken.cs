namespace elastic_app_v3.domain
{
    public sealed record JwtToken(
        string AccessToken,
        string RefreshToken,
        string TokenType, 
        int ExpiresInMinutes
    );
}
