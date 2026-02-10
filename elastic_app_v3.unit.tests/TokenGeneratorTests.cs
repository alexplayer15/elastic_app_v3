using AutoFixture;
using FluentValidation.Results;
using elastic_app_v3.Config;
using elastic_app_v3.Enums;
using elastic_app_v3.Domain;
using elastic_app_v3.Tokens;
using FluentValidation;
using NSubstitute;
using elastic_app_v3.Errors;
using Microsoft.Extensions.Options;

namespace elastic_app_v3.unit.tests
{
    public class TokenGeneratorTests
    {
        private readonly ITokenGenerator _tokenGenerator;

        private readonly Fixture _fixture = new();
        public TokenGeneratorTests()
        {
            var jwtOptions = Options.Create(new JwtConfigOptions
            {
                PrivateKey = "BANANA-MANGO-PINEAPPLE-CHERRY-GUAVA",
                ExpirationInMinutes = 60,
                Issuer = "PINEAPPLE",
                Audience = "MANGO"
            });

            _tokenGenerator = new TokenGenerator(jwtOptions);

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
            var userSchema = _fixture.Create<UserSchema>();

            //Act
            var tokenResult = await _tokenGenerator.Generate(userSchema);

            //Assert
            Assert.True(tokenResult.IsSuccess);
            Assert.NotNull(tokenResult.Value);
            Assert.NotEmpty(tokenResult.Value.AccessToken);
        }
    }
}
