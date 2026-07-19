using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using elastic_app_v3.infrastructure.Config;
using elastic_app_v3.domain.Entities;
using CSharpFunctionalExtensions;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.DTOs;
using elastic_app_v3.domain.Errors;

namespace elastic_app_v3.infrastructure.Tokens
{
    public sealed class TokenGenerator(IOptions<JwtConfigOptions> jwtConfig) : ITokenGenerator
    {
        private readonly JwtConfigOptions _jwtConfig = jwtConfig.Value;  
        public Result<JwtToken, UserError> Generate(User user) //don't think this should be a user error, come back to this
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

            return Result.Success<JwtToken, UserError>(
                new JwtToken(token, 
                string.Empty, 
                "Bearer",
                _jwtConfig.ExpirationInMinutes
                )
            ); //to do: implement refresh token
        }
    }

}
