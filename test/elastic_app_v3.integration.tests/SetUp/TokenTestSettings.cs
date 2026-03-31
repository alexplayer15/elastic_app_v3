using elastic_app_v3.infrastructure.Config;

namespace elastic_app_v3.integration.tests.SetUp;
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
