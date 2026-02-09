using AutoFixture;
using elastic_app_v3.Config;
using elastic_app_v3.Errors;
using elastic_app_v3.Validations;
using FluentValidation.TestHelper;

namespace elastic_app_v3.unit.tests.Validations
{
    public class JwtConfigValidatorTests
    {
        private readonly Fixture _fixture = new();
        private readonly JwtConfigValidator _jwtConfigValidator = new();
        public JwtConfigValidatorTests()
        {
            _fixture.Customize<JwtConfig>(j => j
                .With(j => j.PrivateKey, "BANANA-MANGO-PINEAPPLE-CHERRY-GUAVA")
                .With(j => j.ExpirationInMinutes, 60)
                .With(j => j.Issuer, "MANGO")
                .With(j => j.Audience, "PINEAPPLE")
            );
        }

        [Fact]
        public void GivenEmptyPrivateKey_WhenTestValidate_ThenReturnError()
        {
            var jwtConfig = _fixture.Create<JwtConfig>() with
            {
                PrivateKey = string.Empty
            };

            var result = _jwtConfigValidator.TestValidate(jwtConfig);

            result.ShouldHaveValidationErrorFor(x => x.PrivateKey)
                  .WithErrorMessage(ErrorMessages.PrivateKeyEmpty);
        }

        [Fact]
        public void GivenZeroExpiration_WhenTestValidate_ThenReturnError()
        {
            var jwtConfig = _fixture.Create<JwtConfig>() with
            {
                ExpirationInMinutes = 0
            };

            var result = _jwtConfigValidator.TestValidate(jwtConfig);

            result.ShouldHaveValidationErrorFor(x => x.ExpirationInMinutes)
                  .WithErrorMessage(ErrorMessages.ExpirationZeroOrNegative);
        }

        [Fact]
        public void GivenNegativeExpiration_WhenTestValidate_ThenReturnError()
        {
            var jwtConfig = _fixture.Create<JwtConfig>() with
            {
                ExpirationInMinutes = -5
            };

            var result = _jwtConfigValidator.TestValidate(jwtConfig);

            result.ShouldHaveValidationErrorFor(x => x.ExpirationInMinutes)
                  .WithErrorMessage(ErrorMessages.ExpirationZeroOrNegative);
        }

        [Fact]
        public void GivenTooLongExpiration_WhenTestValidate_ThenReturnError()
        {
            var jwtConfig = _fixture.Create<JwtConfig>() with
            {
                ExpirationInMinutes = 1441
            };

            var result = _jwtConfigValidator.TestValidate(jwtConfig);

            result.ShouldHaveValidationErrorFor(x => x.ExpirationInMinutes)
                  .WithErrorMessage(ErrorMessages.ExpirationTooLong);
        }

        [Fact]
        public void GivenEmptyIssuer_WhenTestValidate_ThenReturnError()
        {
            var jwtConfig = _fixture.Create<JwtConfig>() with
            {
                Issuer = string.Empty
            };

            var result = _jwtConfigValidator.TestValidate(jwtConfig);

            result.ShouldHaveValidationErrorFor(x => x.Issuer)
                  .WithErrorMessage(ErrorMessages.IssuerEmpty);
        }

        [Fact]
        public void GivenEmptyAudience_WhenTestValidate_ThenReturnError()
        {
            var jwtConfig = _fixture.Create<JwtConfig>() with
            {
                Audience = string.Empty
            };

            var result = _jwtConfigValidator.TestValidate(jwtConfig);

            result.ShouldHaveValidationErrorFor(x => x.Audience)
                  .WithErrorMessage(ErrorMessages.AudienceEmpty);
        }
    }
}
