using AutoFixture;
using elastic_app_v3.application.DTOs.Login;
using elastic_app_v3.application.Errors;
using elastic_app_v3.application.Errors.Identity;
using elastic_app_v3.application.Services.Identity;
using elastic_app_v3.domain;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Entities;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using NSubstitute;


namespace elastic_app_v3.unit.tests.Services.Identity;
public class LoginServiceTests
{
    private readonly IUserRepository _mockUserRepository = Substitute.For<IUserRepository>();
    private readonly IValidator<LoginRequest> _mockLoginRequestValidator
        = Substitute.For<IValidator<LoginRequest>>();
    private readonly ITokenGenerator _mockTokenGenerator = Substitute.For<ITokenGenerator>();
    private readonly IPasswordHasher<User> _mockPasswordHasher = Substitute.For<IPasswordHasher<User>>();
    private readonly ILoginService _loginService;

    private readonly Fixture _fixture = new();
    public LoginServiceTests()
    {
        _loginService = new LoginService(
            _mockLoginRequestValidator,
            _mockUserRepository,
            _mockTokenGenerator,
            _mockPasswordHasher
        );

        _fixture.Customize<LoginRequest>(lr => lr
            .With(lr => lr.UserName, "alexplayer15")
            .With(lr => lr.Password, "password")
        );
    }

    [Fact]
    public async Task GivenInvalidLoginRequest_WhenLoginAsync_ThenReturnValidationError()
    {
        //Arrange
        var request = _fixture.Create<LoginRequest>();

        _mockLoginRequestValidator.Validate(request)
            .Returns(new ValidationResult()
            {
                Errors = { new ValidationFailure("UserName", IdentityErrorMessages.UserNameEmpty) }
            });

        //Act
        var loginResult = await _loginService.LoginAsync(request);

        //Assert
        Assert.False(loginResult.IsSuccess);
        Assert.Single(loginResult.Errors);
        Assert.IsType<ValidationError>(loginResult.Errors[0]);
    }

    [Fact]
    public async Task GivenLoginRequestWithRepositoryFailure_WhenLoginAsync_ThenReturnRepositoryError()
    {
        //Arrange
        var request = _fixture.Create<LoginRequest>();
        _mockLoginRequestValidator.Validate(request)
        .Returns(new ValidationResult());

        _mockUserRepository.GetUserByUsernameAsync(request.UserName)
            .Returns(Result.Fail(new UserDoesNotExistError()));

        //Act
        var loginResult = await _loginService.LoginAsync(request);

        //Assert
        Assert.False(loginResult.IsSuccess);
        Assert.Single(loginResult.Errors);
        Assert.IsType<UserDoesNotExistError>(loginResult.Errors[0]);
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
            .Returns(Result.Ok(user));

        _mockPasswordHasher.VerifyHashedPassword(
            Arg.Any<User>(), user.PasswordHash, request.Password)
            .Returns(PasswordVerificationResult.Success);

        var accessToken = Guid.NewGuid().ToString();
        _mockTokenGenerator.Generate(Arg.Any<User>())
            .Returns(Result.Ok(new JwtToken
            (accessToken, string.Empty, "Bearer", 60))
        );

        //Act
        var loginResult = await _loginService.LoginAsync(request);

        //Assert
        Assert.True(loginResult.IsSuccess);
        Assert.NotNull(loginResult.Value);
        Assert.Equal(accessToken, loginResult.Value.AccessToken);
        Assert.Equal(60, loginResult.Value.ExpiresInMinutes);
        Assert.Equal("Bearer", loginResult.Value.TokenType);
    }

}
