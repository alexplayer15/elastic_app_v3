using AutoFixture;
using FluentValidation;
using NSubstitute;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using elastic_app_v3.infrastructure.Repositories;
using elastic_app_v3.application.Services;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.application.DTOs;
using elastic_app_v3.application.Errors;
using elastic_app_v3.infrastructure.Models;
using elastic_app_v3.domain.Result;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain;

namespace elastic_app_v3.unit.tests
{
    public class UserServiceTests
    {
        private readonly IUserRepository _mockUserRepository = Substitute.For<IUserRepository>();
        private readonly IValidator<SignUpRequest> _mockSignUpRequestValidator
            = Substitute.For<IValidator<SignUpRequest>>();
        private readonly IValidator<LoginRequest> _mockLoginRequestValidator
            = Substitute.For<IValidator<LoginRequest>>();
        private readonly ITokenGenerator _mockTokenGenerator = Substitute.For<ITokenGenerator>();
        private readonly IPasswordHasher<User> _mockPasswordHasher = Substitute.For<IPasswordHasher<User>>();

        private readonly IUserService _userService;

        private readonly Fixture _fixture = new();
        public UserServiceTests()
        {
            _userService = new UserService(
                _mockUserRepository,
                _mockSignUpRequestValidator,
                _mockLoginRequestValidator,
                _mockPasswordHasher,
                _mockTokenGenerator
            );

            _fixture.Customize<SignUpRequest>(c => c
                .With(x => x.FirstName, "Alex")
                .With(x => x.LastName, "Player")
                .With(x => x.UserName, "alexplayer15")
                .With(x => x.Password, "password") 
                .With(x => x.ReEnteredPassword, "password")
            );

            _fixture.Customize<LoginRequest>(lr => lr
                .With(lr => lr.UserName, "alexplayer15")
                .With(lr => lr.Password, "password")
            );
        }

        [Fact]
        public async Task GivenValidSignUpRequest_WhenSignUpAsync_ThenReturnUserId()
        {
            //Arrange
            var request = _fixture.Create<SignUpRequest>();

            _mockSignUpRequestValidator.Validate(request)
                .Returns(new ValidationResult());

            var userId = _fixture.Create<Guid>();

            _mockPasswordHasher.HashPassword(Arg.Any<User>(), request.Password) 
                .Returns("hashedPassword");

            _mockUserRepository.AddAsync(Arg.Any<User>())
                .Returns(Result<Guid>.Success(userId));

            //Act
            var signUpResult = await _userService.SignUpAsync(request);

            //Assert
            Assert.True(signUpResult.IsSuccess);
            Assert.Equal(userId, signUpResult.Value.UserId);
        }

        [Fact]
        public async Task GivenInValidSignUpRequest_WhenSignUpAsync_ThenReturnValidationError()
        {
            //Arrange
            var request = _fixture.Create<SignUpRequest>() with
            {
                UserName = string.Empty
            };

            _mockSignUpRequestValidator.Validate(request)
            .Returns(new ValidationResult()
            {
                Errors = { new ValidationFailure("UserName", ErrorMessages.UserNameEmpty) }
            });

            //Act
            var signUpResult = await _userService.SignUpAsync(request);

            //Assert
            Assert.False(signUpResult.IsSuccess);
            Assert.NotNull(signUpResult.Error);
            Assert.Equal("Validation.ValidationError", signUpResult.Error.Code); //could change this code to reflect field which failed
            Assert.Equal(ErrorCategory.ValidationError, signUpResult.Error.ErrorCategory); //do we need code and category? How would consumer use the code?
            Assert.Equal(ErrorMessages.UserNameEmpty, signUpResult.Error.Message);
        }

        [Fact]
        public async Task GivenSignUpRequestWithRepositoryFailure_WhenSignUpAsync_ThenReturnRepositoryError()
        {
            //Arrange
            var request = _fixture.Create<SignUpRequest>();

            _mockSignUpRequestValidator.Validate(request)
            .Returns(new ValidationResult());

            _mockPasswordHasher.HashPassword(Arg.Any<User>(), request.Password)
                .Returns("hashedPassword");

            _mockUserRepository.AddAsync(Arg.Any<User>())
                .Returns(Result<Guid>.Failure(UserErrors.UserAlreadyExistsError("alexplayer15")));

            //Act
            var signUpResult = await _userService.SignUpAsync(request);

            //Assert
            Assert.False(signUpResult.IsSuccess);
            Assert.NotNull(signUpResult.Error);
            Assert.Equal("User.AlreadyExists", signUpResult.Error.Code);
            Assert.Equal(ErrorCategory.UserAlreadyExists, signUpResult.Error.ErrorCategory);
            Assert.Equal("User with username: alexplayer15 already exists", signUpResult.Error.Message);
        }

        [Fact]
        public async Task GivenInvalidLoginRequest_WhenLoginAsync_ThenReturnValidationError()
        {
            //Arrange
            var request = _fixture.Create<LoginRequest>();

            _mockLoginRequestValidator.Validate(request)
            .Returns(new ValidationResult()
            {
                Errors = { new ValidationFailure("UserName", ErrorMessages.UserNameEmpty) }
            });

            //Act
            var loginResult = await _userService.LoginAsync(request);

            //Assert
            Assert.False(loginResult.IsSuccess);
            Assert.NotNull(loginResult.Error);
            Assert.Equal(ErrorCategory.ValidationError, loginResult.Error.ErrorCategory);
            Assert.Equal(ErrorMessages.UserNameEmpty, loginResult.Error.Message);
        }

        [Fact]
        public async Task GivenLoginRequestWithRepositoryFailure_WhenLoginAsync_ThenReturnRepositoryError()
        {
            //Arrange
            var request = _fixture.Create<LoginRequest>();
            _mockLoginRequestValidator.Validate(request)
            .Returns(new ValidationResult());

            _mockUserRepository.GetUserByUsernameAsync(request.UserName)
                .Returns(Result<User>.Failure(UserErrors.UserDoesNotExistError));

            //Act
            var loginResult = await _userService.LoginAsync(request);

            //Assert
            Assert.False(loginResult.IsSuccess);
            Assert.NotNull(loginResult.Error);
            Assert.Equal(ErrorCategory.UserDoesNotExist, loginResult.Error.ErrorCategory);
        }

        [Fact]
        public async Task GivenValidLoginRequest_WhenLoginAsync_ThenReturnSuccessAndToken()
        {
            //Arrange
            var request = _fixture.Create<LoginRequest>();
            _mockLoginRequestValidator.Validate(request)
            .Returns(new ValidationResult());

            var userId = Guid.NewGuid();
            var user = _fixture.Build<User>()
                .With(u => u.Id, userId)
                .With(u => u.FirstName, "Alex")
                .With(u => u.LastName, "Player")
                .With(u => u.UserName, request.UserName)
                .With(u => u.PasswordHash, request.Password)
                .Create();

            _mockUserRepository.GetUserByUsernameAsync(request.UserName)
                .Returns(Result<User>.Success(user));

            _mockPasswordHasher.VerifyHashedPassword(
                Arg.Any<User>(), user.PasswordHash, request.Password)
                .Returns(PasswordVerificationResult.Success);

            var accessToken = Guid.NewGuid().ToString();
            _mockTokenGenerator.Generate(Arg.Any<User>())
                .Returns(Result<JwtToken>.Success(new JwtToken
                (accessToken, string.Empty, "Bearer", 60)
                ));

            //Act
            var loginResult = await _userService.LoginAsync(request);

            //Assert
            Assert.True(loginResult.IsSuccess);
            Assert.NotNull(loginResult.Value);
            Assert.Equal(accessToken, loginResult.Value.AccessToken);
            Assert.Equal(60, loginResult.Value.ExpiresInMinutes);
            Assert.Equal("Bearer", loginResult.Value.TokenType);
        }
    }
}
