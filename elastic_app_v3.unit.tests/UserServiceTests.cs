using AutoFixture;
using elastic_app_v3.DTOs;
using elastic_app_v3.Repositories;
using elastic_app_v3.Services;
using elastic_app_v3.Tokens;
using FluentValidation;
using NSubstitute;
using FluentValidation.Results;
using elastic_app_v3.Domain;
using elastic_app_v3.Errors;

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

        private readonly IUserService _userService;

        private readonly Fixture _fixture = new();
        public UserServiceTests()
        {
            _userService = new UserService(
                _mockUserRepository,
                _mockSignUpRequestValidator,
                _mockLoginRequestValidator,
                _mockTokenGenerator
            );

            _fixture.Customize<SignUpRequest>(c => c
                .With(x => x.FirstName, "Alex")
                .With(x => x.LastName, "Player")
                .With(x => x.UserName, "alexplayer15")
                .With(x => x.Password, "password") 
                .With(x => x.ReEnteredPassword, "password")
            );

            _fixture.Customize<User>(u => u
                .With(u => u.FirstName, "Alex")
                .With(u => u.LastName, "Player")
                .With(u => u.UserName, "alexplayer15")
                .With(u => u.PasswordHash, "password")
            );
        }

        [Fact]
        public async Task GivenValidSignUpRequest_WhenSignUpAsync_ThenReturnUserId()
        {
            //Arrange
            var request = _fixture.Create<SignUpRequest>();
            _mockSignUpRequestValidator.Validate(request)
            .Returns(new ValidationResult());

            var user = _fixture.Create<User>();
            var userId = _fixture.Create<Guid>();
            _mockUserRepository.AddAsync(user)
                .Returns(Result<Guid>.Success(userId));

            //Act
            var signUpResult = await _userService.SignUpAsync(request);

            //Assert
            Assert.True(signUpResult.IsSuccess);
            Assert.Equal(userId, signUpResult.Value.UserId);
        }
    }
}
