using System.Security.Claims;
using System.Text;
using elastic_app_v3.infrastructure.Config;
using elastic_app_v3.integration.tests.SetUp;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace elastic_app_v3.integration.tests.InfraHelpers;
public class TokenHelper
{
    private static readonly JwtConfigOptions _jwtConfig = TokenTestSettings.SetJwtConfigTestSettings();
    public static string GenerateTestToken(Guid userId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.PrivateKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()) //needs to match what endpoint expects
            ]),
            Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationInMinutes),
            SigningCredentials = credentials,
            Issuer = _jwtConfig.Issuer,
            Audience = _jwtConfig.Audience
        };

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(descriptor);
    }
}
