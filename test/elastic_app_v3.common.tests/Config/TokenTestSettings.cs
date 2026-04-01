using elastic_app_v3.infrastructure.Config;

namespace elastic_app_v3.common.tests.Config;
internal static class TokenTestSettings
{
    public static JwtConfigOptions SetJwtConfigTestSettings() => new()
    {
        PrivateKey = "BANANA-MANGO-PINEAPPLE-CHERRY-GUAVA",
        ExpirationInMinutes = 60,
        Issuer = "PINEAPPLE",
        Audience = "MANGO"
    };
}
