using System.Security.Claims;
using System.Text;
using System.Text.Json;
using elastic_app_v3.Config;
using elastic_app_v3.Domain;
using elastic_app_v3.DTOs;
using elastic_app_v3.Errors;
using elastic_app_v3.SecretsManager;
using FluentValidation;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace elastic_app_v3.Tokens
{
    public sealed class TokenGenerator(
        ISecretsManagerClient secretsManagerClient,
        IValidator<JwtConfig> jwtConfigValidator) : ITokenGenerator
    {
        private readonly ISecretsManagerClient _secretsManagerClient = secretsManagerClient;
        private readonly IValidator<JwtConfig> _jwtConfigValidator = jwtConfigValidator;
        public async Task<Result<LoginResponse>> Generate(UserSchema userSchema) //currently carrying data not needed 
        {
            var stringJwtConfig = await _secretsManagerClient.GetSecretString("JwtConfig");

            var jwtConfig = TryDeserializeJwtConfig(stringJwtConfig);
            var validationResult = _jwtConfigValidator.Validate(jwtConfig);

            if (!validationResult.IsValid)
            {
                var errorDescription = string.Join("; ",
                    validationResult.Errors.Select(e => e.ErrorMessage));

                return Result<LoginResponse>.Failure(ValidationErrors.JwtConfigValidationError(errorDescription)); //feels off to treat these validations as errors but deserialization issues as exceptions...
            }

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

        private static JwtConfig TryDeserializeJwtConfig(string jwtConfig)
        {
            try
            {
                var config = JsonSerializer.Deserialize<JwtConfig>(jwtConfig)
                    ?? throw new InvalidOperationException("JWT config is empty. The JSON string was valid but resulted in a null object.");

                return config;
            }
            catch (ArgumentNullException ex)
            {
                throw new InvalidOperationException("JWT config string was null, cannot deserialize", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    "Failed to deserialize JWT config: the JSON is malformed or missing required fields", ex);
            }
        }
    }
}
