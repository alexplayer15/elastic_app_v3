using AutoFixture;
using FluentValidation.TestHelper;
using elastic_app_v3.application.Validations;
using elastic_app_v3.application.DTOs.Login;
using elastic_app_v3.application.Errors.Identity;

namespace elastic_app_v3.unit.tests.Validations
{
    public class LoginRequestValidatorTests
    {
        private readonly Fixture _fixture = new();
        private readonly LoginRequestValidator _loginRequestValidator = new();
        public LoginRequestValidatorTests()
        {
            _fixture.Customize<LoginRequest>(r => r
                .With(r => r.UserName, "alexplayer15")
                .With(r => r.Password, "password")
            );
        }

        [Fact]
        public void GivenEmptyUserName_WhenTestValidate_ThenReturnErrorForUserName()
        {
            // Arrange
            var loginRequest = _fixture.Create<LoginRequest>() with
            {
                UserName = string.Empty
            };

            // Act
            var result = _loginRequestValidator.TestValidate(loginRequest);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.UserName)
                  .WithErrorMessage(IdentityErrorMessages.UserNameEmpty);
        }

        [Fact]
        public void GivenTooShortUserName_WhenTestValidate_ThenReturnErrorForUserName()
        {
            var loginRequest = _fixture.Create<LoginRequest>() with
            {
                UserName = "a"
            };

            var result = _loginRequestValidator.TestValidate(loginRequest);

            result.ShouldHaveValidationErrorFor(r => r.UserName)
                .WithErrorMessage(IdentityErrorMessages.UserNameTooShortMessage());
        }

        [Fact]
        public void GivenTooLongUserName_WhenTestValidate_ThenReturnErrorForUserName()
        {
            var loginRequest = _fixture.Create<LoginRequest>() with
            {
                UserName = new string('a', 23)
            };

            var result = _loginRequestValidator.TestValidate(loginRequest);

            result.ShouldHaveValidationErrorFor(r => r.UserName)
                .WithErrorMessage(IdentityErrorMessages.UserNameTooLongMessage());
        }

        [Fact]
        public void GivenEmptyPassword_WhenTestValidate_ThenReturnErrorForPassword()
        {
            var loginRequest = _fixture.Create<LoginRequest>() with
            {
                Password = string.Empty
            };

            var result = _loginRequestValidator.TestValidate(loginRequest);

            result.ShouldHaveValidationErrorFor(r => r.Password)
                .WithErrorMessage(IdentityErrorMessages.PasswordEmpty);
        }

        [Fact]
        public void GivenTooShortPassword_WhenTestValidate_ThenReturnErrorForPassword()
        {
            var loginRequest = _fixture.Create<LoginRequest>() with
            {
                Password = "a"
            };

            var result = _loginRequestValidator.TestValidate(loginRequest);

            result.ShouldHaveValidationErrorFor(r => r.Password)
                .WithErrorMessage(IdentityErrorMessages.PasswordTooShortMessage());
        }

        [Fact]
        public void GivenTooLongPassword_WhenTestValidate_ThenReturnErrorForPassword()
        {
            var loginRequest = _fixture.Create<LoginRequest>() with
            {
                Password = new string('a', 23)
            };

            var result = _loginRequestValidator.TestValidate(loginRequest);

            result.ShouldHaveValidationErrorFor(r => r.Password)
                .WithErrorMessage(IdentityErrorMessages.PasswordTooLongMessage());
        }
    }
}
