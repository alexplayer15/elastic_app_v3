using AutoFixture;
using FluentValidation.TestHelper;
using elastic_app_v3.DTOs;
using elastic_app_v3.Validations;
using elastic_app_v3.Errors;

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
        public void GivenEmptyFirstName_WhenTestValidate_ThenReturnErrorForFirstName()
        {
            //Arrange
            var signUpRequest = _fixture.Create<SignUpRequest>();
            signUpRequest.FirstName = string.Empty;

            //Act
            var result = _signUpRequestValidator.TestValidate(signUpRequest);

            //Assert
            result.ShouldHaveValidationErrorFor(r => r.FirstName)
                .WithErrorMessage(ErrorMessages.FirstNameEmpty);
        }

        [Fact]
        public void GivenNonAlphabeticalFirstName_WhenTestValidate_ThenReturnErrorForFirstName()
        {
            //Arrange
            var signUpRequest = _fixture.Create<SignUpRequest>();
            signUpRequest.FirstName = "AL3X";

            //Act
            var result = _signUpRequestValidator.TestValidate(signUpRequest);

            //Assert
            result.ShouldHaveValidationErrorFor(r => r.FirstName)
                .WithErrorMessage(ErrorMessages.FirstNameNonAlphabetical);
        }

        [Fact]
        public void GivenEmptyLastName_WhenTestValidate_ThenReturnErrorForLastName()
        {
            //Arrange
            var signUpRequest = _fixture.Create<SignUpRequest>();
            signUpRequest.LastName = string.Empty;

            //Act
            var result = _signUpRequestValidator.TestValidate(signUpRequest);

            //Assert
            result.ShouldHaveValidationErrorFor(r => r.LastName)
                .WithErrorMessage(ErrorMessages.LastNameEmpty);
        }

        [Fact]
        public void GivenNonAlphabeticalLastName_WhenTestValidate_ThenReturnErrorForLastName()
        {
            //Arrange
            var signUpRequest = _fixture.Create<SignUpRequest>();
            signUpRequest.LastName = "PL4Y3R";

            //Act
            var result = _signUpRequestValidator.TestValidate(signUpRequest);

            //Assert
            result.ShouldHaveValidationErrorFor(r => r.LastName)
                .WithErrorMessage(ErrorMessages.LastNameNonAlphabetical);
        }

        [Fact]
        public void GivenEmptyUserName_WhenTestValidate_ThenReturnErrorForUserName()
        {
            // Arrange
            var signUpRequest = _fixture.Create<SignUpRequest>();
            signUpRequest.UserName = string.Empty;

            // Act
            var result = _signUpRequestValidator.TestValidate(signUpRequest);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.UserName)
                  .WithErrorMessage(ErrorMessages.UserNameEmpty);
        }

        [Fact]
        public void GivenTooShortUserName_WhenTestValidate_ThenReturnErrorForUserName()
        {
            // Arrange
            var signUpRequest = _fixture.Create<SignUpRequest>();
            signUpRequest.UserName = "a";

            // Act
            var result = _signUpRequestValidator.TestValidate(signUpRequest);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.UserName)
                  .WithErrorMessage(ErrorMessages.UserNameTooShortMessage());
        }

        [Fact]
        public void GivenTooLongUserName_WhenTestValidate_ThenReturnErrorForUserName()
        {
            // Arrange
            var signUpRequest = _fixture.Create<SignUpRequest>();
            signUpRequest.UserName = new string('a', 23);

            // Act
            var result = _signUpRequestValidator.TestValidate(signUpRequest);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.UserName)
                  .WithErrorMessage(ErrorMessages.UserNameTooLongMessage());
        }

        [Fact]
        public void GivenTooShortPassword_WhenTestValidate_ThenReturnErrorForPassword()
        {
            // Arrange
            var signUpRequest = _fixture.Create<SignUpRequest>();
            signUpRequest.Password = "a";

            // Act
            var result = _signUpRequestValidator.TestValidate(signUpRequest);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.Password)
                  .WithErrorMessage(ErrorMessages.PasswordTooShortMessage());
        }

        [Fact]
        public void GivenTooLongPassword_WhenTestValidate_ThenReturnErrorForPassword()
        {
            // Arrange
            var signUpRequest = _fixture.Create<SignUpRequest>();
            signUpRequest.Password = new string('a', 23);

            // Act
            var result = _signUpRequestValidator.TestValidate(signUpRequest);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.Password)
                  .WithErrorMessage(ErrorMessages.PasswordTooLongMessage());
        }

        [Fact]
        public void GivenEmptyReEnteredPassword_WhenTestValidate_ThenReturnErrorForReEnteredPassword()
        {
            // Arrange
            var signUpRequest = _fixture.Create<SignUpRequest>();
            signUpRequest.ReEnteredPassword = string.Empty;

            // Act
            var result = _signUpRequestValidator.TestValidate(signUpRequest);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.ReEnteredPassword)
                  .WithErrorMessage(ErrorMessages.ReEnteredPasswordEmpty);
        }

        [Fact]
        public void GivenNonMatchingReEnteredPassword_WhenTestValidate_ThenReturnErrorForReEnteredPassword()
        {
            // Arrange
            var signUpRequest = _fixture.Create<SignUpRequest>();
            signUpRequest.Password = "password";
            signUpRequest.ReEnteredPassword = "nonMatchingPassword";

            // Act
            var result = _signUpRequestValidator.TestValidate(signUpRequest);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.ReEnteredPassword)
                  .WithErrorMessage(ErrorMessages.ReEnteredPasswordNotMatching);
        }
    }
}