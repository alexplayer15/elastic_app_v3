using System.Security.Claims;
using System.Text;
using System.Text.Json;
using elastic_app_v3.Config;
using elastic_app_v3.Domain;
using elastic_app_v3.DTOs;
using elastic_app_v3.Errors;
using elastic_app_v3.SecretsManager;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace elastic_app_v3.Tokens
{
    public sealed class TokenGenerator(ISecretsManagerClient secretsManagerClient) : ITokenGenerator
    {
        private readonly ISecretsManagerClient _secretsManagerClient = secretsManagerClient;
        public async Task<Result<LoginResponse>> Generate(UserSchema userSchema)
        {
            var stringJwtConfig = await _secretsManagerClient.GetSecretString("JwtConfig");

            var jwtConfig = JsonSerializer.Deserialize<JwtConfig>(stringJwtConfig);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig!.PrivateKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, userSchema.Id.ToString())
                ]),
                Expires = DateTime.UtcNow.AddMinutes(jwtConfig.ExpirationInMinutes),
                SigningCredentials = credentials,
                Issuer = jwtConfig.Issuer,
                Audience = jwtConfig.Audience,
            };

            var handler = new JsonWebTokenHandler();

            string token = handler.CreateToken(tokenDescriptor);

            return Result<LoginResponse>.Success(
                new LoginResponse(
                    token, 
                    string.Empty, 
                    "Bearer", 
                    jwtConfig.ExpirationInMinutes
                ));
        }
    }
}
