namespace elastic_app_v3.Config
{
    public sealed record JwtConfig(
        string PrivateKey,
        int ExpirationInMinutes,
        string Issuer,
        string Audience
    );
}
