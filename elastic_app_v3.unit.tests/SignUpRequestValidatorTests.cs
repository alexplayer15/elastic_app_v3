using AutoFixture;
using FluentValidation.TestHelper;
using elastic_app_v3.DTOs;
using elastic_app_v3.Validations;

namespace elastic_app_v3.unit.tests
{
    public class SignUpRequestValidatorTests
    {
        private readonly Fixture _fixture = new();
        private readonly SignUpRequestValidator _signUpRequestValidator = new();

        public SignUpRequestValidatorTests()
        {
            _fixture.Customize<SignUpRequest>(r => r
                .With(r => r.FirstName, "Alex")
                .With(r => r.LastName, "Player")
                .With(r => r.UserName, "alexplayer15")
                .With(r => r.Password, "password")
                .With(r => r.ReEnteredPassword, "password")
            );
        }

        [Fact]
        public void GivenInvalidFirstName_WhenValidate_ThenReturnErrorForFirstName()
        {
            //Arrange
            var signUpRequest = _fixture.Create<SignUpRequest>();
            signUpRequest.FirstName = string.Empty;

            //Act
            var result = _signUpRequestValidator.TestValidate(signUpRequest);

            //Assert
            result.ShouldHaveValidationErrorFor(r => r.FirstName);
        }
    }
}