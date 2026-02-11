using System.Security.Claims;
using System.Text;
using elastic_app_v3.Config;
using elastic_app_v3.Domain;
using elastic_app_v3.DTOs;
using elastic_app_v3.Errors;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace elastic_app_v3.Tokens
{
    public sealed class TokenGenerator(IOptions<JwtConfigOptions> jwtConfig) : ITokenGenerator
    {
        private readonly JwtConfigOptions _jwtConfig = jwtConfig.Value;  
        public Task<Result<LoginResponse>> Generate(User user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtConfig.PrivateKey));

            var credentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
                ]),
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationInMinutes),
                SigningCredentials = credentials,
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience,
            };

            var handler = new JsonWebTokenHandler();
            var token = handler.CreateToken(descriptor);

            return Task.FromResult(
                Result<LoginResponse>.Success(
                    new LoginResponse(token, "", "Bearer",
                        _jwtConfig.ExpirationInMinutes)));
        }
    }

}
