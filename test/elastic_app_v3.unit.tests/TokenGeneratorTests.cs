using AutoFixture;
using elastic_app_v3.infrastructure.Config;
using elastic_app_v3.infrastructure.Tokens;
using Microsoft.Extensions.Options;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.domain.Abstractions;

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

            _fixture.Customize<User>(u => u
                .With(u => u.FirstName, "Alex")
                .With(u => u.LastName, "Player")
                .With(u => u.UserName, "alexplayer15")
                .With(u => u.PasswordHash, "password")
            );
        }

        [Fact]
        public void GivenValidJwtConfig_WhenGenerate_ThenReturnAccessToken()
        {
            //Arrange
            var user = _fixture.Create<User>();

            //Act
            var tokenResult = _tokenGenerator.Generate(user);

            //Assert
            Assert.True(tokenResult.IsSuccess);
            Assert.NotNull(tokenResult.Value);
            Assert.NotEmpty(tokenResult.Value.AccessToken);
        }
    }
}
