using AutoFixture;
using NSubstitute;
using Microsoft.AspNetCore.Identity;
using elastic_app_v3.domain.Entities;
using CSharpFunctionalExtensions;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.application.Services.Identity;
using elastic_app_v3.application.DTOs.SignUp;
using elastic_app_v3.domain.Errors;
using elastic_app_v3.domain.Errors.Identity;

namespace elastic_app_v3.unit.tests.Services.Identity;
public class UserServiceTests
{
    private readonly IUserRepository _mockUserRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher<User> _mockPasswordHasher = Substitute.For<IPasswordHasher<User>>();
    private readonly IUserService _userService;

    private readonly Fixture _fixture = new();
    public UserServiceTests()
    {
        _userService = new UserService(
            _mockUserRepository,
            _mockPasswordHasher
        );

        _fixture.Customize<SignUpRequest>(c => c
            .With(x => x.FirstName, "Alex")
            .With(x => x.LastName, "Player")
            .With(x => x.UserName, "alexplayer15")
            .With(x => x.Password, "password") 
            .With(x => x.ReEnteredPassword, "password")
        );
    }

    [Fact]
    public async Task GivenValidSignUpRequest_WhenSignUpAsync_ThenReturnUserId()
    {
        //Arrange
        var request = _fixture.Create<SignUpRequest>();

        _mockPasswordHasher.HashPassword(Arg.Any<User>(), request.Password) 
            .Returns("hashedPassword");

        _mockUserRepository.AddAsync(Arg.Any<User>(), CancellationToken.None)
            .Returns(UnitResult.Success<UserError>());

        //Act
        var signUpResult = await _userService.SignUpAsync(request, CancellationToken.None);

        //Assert
        Assert.True(signUpResult.IsSuccess);
    }

    [Fact]
    public async Task GivenSignUpRequestWithRepositoryFailure_WhenSignUpAsync_ThenReturnRepositoryError()
    {
        //Arrange
        var request = _fixture.Create<SignUpRequest>();

        _mockPasswordHasher.HashPassword(Arg.Any<User>(), request.Password)
            .Returns("hashedPassword");

        _mockUserRepository.AddAsync(Arg.Any<User>(), CancellationToken.None)
            .Returns(UnitResult.Failure<UserError>(new UserAlreadyExistsError()));

        //Act
        var signUpResult = await _userService.SignUpAsync(request, CancellationToken.None);

        //Assert
        Assert.False(signUpResult.IsSuccess);
        Assert.NotNull(signUpResult.Error);
        Assert.IsType<UserAlreadyExistsError>(signUpResult.Error);
    }

    [Fact]
    public async Task GivenExistingUser_WhenGetUserByIdAsync_ThenReturnUser()
    {
        //Arrange
        var userId = _fixture.Create<Guid>();
        var user = _fixture.Create<User>();

        _mockUserRepository.GetUserByIdAsync(userId, CancellationToken.None)
            .Returns(Result.Success<User, UserError>(user));

        //Act
        var result = await _userService.GetUserByIdAsync(userId, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(result.Value.FirstName, user.FirstName);
        Assert.Equal(result.Value.LastName, user.LastName);
        Assert.Equal(result.Value.UserName, user.UserName);
    }

    [Fact]
    public async Task GivenNonExistingUser_WhenGetUserByIdAsync_ThenReturnUserDoesNotExistError()
    {
        //Arrange
        var userId = _fixture.Create<Guid>();

        _mockUserRepository.GetUserByIdAsync(userId, CancellationToken.None)
            .Returns(Result.Failure<User, UserError>(new UserDoesNotExistError()));

        //Act
        var result = await _userService.GetUserByIdAsync(userId, CancellationToken.None);

        //Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.IsType<UserDoesNotExistError>(result.Error);
    }
}
