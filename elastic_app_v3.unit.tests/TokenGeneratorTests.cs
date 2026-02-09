using AutoFixture;
using FluentValidation.Results;
using elastic_app_v3.Config;
using elastic_app_v3.Enums;
using elastic_app_v3.Domain;
using elastic_app_v3.SecretsManager;
using elastic_app_v3.Tokens;
using FluentValidation;
using NSubstitute;
using elastic_app_v3.Errors;

namespace elastic_app_v3.unit.tests
{
    public class TokenGeneratorTests
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ISecretsManagerClient _mockSecretsManagerClient = Substitute.For<ISecretsManagerClient>();
        private readonly IValidator<JwtConfig> _mockJwtConfigValidator = Substitute.For<IValidator<JwtConfig>>();

        private readonly Fixture _fixture = new();
        public TokenGeneratorTests()
        {
           _tokenGenerator = new TokenGenerator(
               _mockSecretsManagerClient,
               _mockJwtConfigValidator
           );

            _fixture.Customize<UserSchema>(u => u
                .With(u => u.Id, Guid.NewGuid())
                .With(u => u.FirstName, "Alex")
                .With(u => u.LastName, "Player")
                .With(u => u.UserName, "alexplayer15")
                .With(u => u.PasswordHash, "password")
                .With(u => u.CreatedAt, DateTime.UtcNow)
            );
        }

        [Fact]
        public async Task GivenValidJwtConfig_WhenGenerate_ThenReturnAccessToken()
        {
            //Arrange
            var secretJwtConfig = $@"
            {{
                ""PrivateKey"": ""BANANA-MANGO-PINEAPPLE-CHERRY-GUAVA"",
                ""ExpirationInMinutes"": 60,
                ""Issuer"": ""PINEAPPLE"",
                ""Audience"": ""MANGO""
            }}";

            _mockSecretsManagerClient.GetSecretString(Arg.Any<string>())
                .Returns(secretJwtConfig);

            _mockJwtConfigValidator.Validate(Arg.Any<JwtConfig>())
                .Returns(new ValidationResult());

            var userSchema = _fixture.Create<UserSchema>();

            //Act
            var tokenResult = await _tokenGenerator.Generate(userSchema);

            //Assert
            Assert.True(tokenResult.IsSuccess);
            Assert.NotNull(tokenResult.Value);
            Assert.NotEmpty(tokenResult.Value.AccessToken);
        }

        [Fact]
        public async Task GivenInValidJwtConfig_WhenGenerate_ThenReturnValidationError()
        {
            //Arrange
            var secretJwtConfig = $@"
            {{
                ""ExpirationInMinutes"": 60,
                ""Issuer"": ""PINEAPPLE"",
                ""Audience"": ""MANGO""
            }}";

            _mockSecretsManagerClient.GetSecretString(Arg.Any<string>())
                .Returns(secretJwtConfig);

            var userSchema = _fixture.Create<UserSchema>();

            _mockJwtConfigValidator.Validate(Arg.Any<JwtConfig>())
                .Returns(new ValidationResult()
                {
                    Errors = { new ValidationFailure("PrivateKey", ErrorMessages.PrivateKeyEmpty) }
                });

            //Act
            var tokenResult = await _tokenGenerator.Generate(userSchema);

            //Assert
            Assert.False(tokenResult.IsSuccess);
            Assert.NotNull(tokenResult.Error);
            Assert.Equal(ErrorCategory.JwtConfigValidation, tokenResult.Error.ErrorCategory);
        }

        [Fact]
        public async Task GivenMalformedJwtConfig_WhenGenerate_ThenThrowException()
        {
            //Arrange
            var secretJwtConfig = string.Empty;

            _mockSecretsManagerClient.GetSecretString(Arg.Any<string>())
                .Returns(secretJwtConfig);

            var userSchema = _fixture.Create<UserSchema>();

            //Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _tokenGenerator.Generate(userSchema)
            );

            Assert.Equal(
                "Failed to deserialize JWT config: the JSON is malformed or missing required fields",
                ex.Message
            );
        }

        [Fact]
        public async Task GivenNullJwtConfig_WhenGenerate_ThenThrowException()
        {
            //Arrange
            string secretJwtConfig = null!;

            _mockSecretsManagerClient.GetSecretString(Arg.Any<string>())
                .Returns(secretJwtConfig);

            var userSchema = _fixture.Create<UserSchema>();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _tokenGenerator.Generate(userSchema)
            );

            Assert.Equal(
                "JWT config string was null, cannot deserialize",
                ex.Message
            );
        }
    }
}
