using AutoFixture;
using elastic_app_v3.application.DTOs;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.domain.Result;
using elastic_app_v3.integration.tests.SetUp;
using System.Net;

namespace elastic_app_v3.integration.tests.UserSignUpTests
{
    [Collection(TestCollectionConstants.IntegrationTestCollectionName)]
    public class UserSignUpUnhappyPathTests
    {
        private readonly ApiClient _client;
        private readonly Fixture _fixture = new();
        private readonly UserDbTestHelper _userDbTestHelper = new();
        public UserSignUpUnhappyPathTests(IntegrationTestFixture fixture)
        {
            _client = new ApiClient(fixture.Client);

            var maxUsernameLength = 22;
            //GUID length with N is 32 chars
            var username = $"alexplayer15_{Guid.NewGuid():N}"[..maxUsernameLength];
            _fixture.Customize<SignUpRequest>(c => c
                .With(x => x.FirstName, "Alex")
                .With(x => x.LastName, "Player")
                .With(x => x.UserName, username)
                .With(x => x.Password, "password")
                .With(x => x.ReEnteredPassword, "password")
            );
        }

        [Fact]
        public async Task GivenUserAlreadyExists_WhenSendUserSignUpRequest_ThenReturn409AndUserExistsError()
        {
            //Arrange
            var requestBody = _fixture.Create<SignUpRequest>();

            var user = _fixture.Build<User>()
                .With(u => u.FirstName, requestBody.FirstName)
                .With(u => u.LastName, requestBody.LastName)
                .With(u => u.UserName, requestBody.UserName)
                .With(u => u.PasswordHash, requestBody.Password)
                .Create();

            await _userDbTestHelper.AddTestUserAsync(user, requestBody.Password);

            //Act
            var httpResponse = await _client.SendUserSignupRequest(requestBody);

            //Assert
            Assert.Equal(HttpStatusCode.Conflict, httpResponse.StatusCode);
            var error = await _client.GetErrorResponse(httpResponse);
            Assert.NotNull(error);
            Assert.Equal(ErrorCategory.UserAlreadyExists, error.ErrorCategory);
            Assert.Equal("User.AlreadyExists", error.Code);
        }

        [Fact]
        public async Task GivenInValidUserSignUpRequest_WhenSendUserSignUpRequest_ThenReturn400AndValidationError()
        {
            //Arrange
            var requestBody = _fixture.Create<SignUpRequest>() with
            {
                FirstName = string.Empty
            };

            ///Act
            var httpResponse = await _client.SendUserSignupRequest(requestBody);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
            var error = await _client.GetErrorResponse(httpResponse);
            Assert.NotNull(error);
            Assert.Equal(ErrorCategory.ValidationError, error.ErrorCategory);
            Assert.Equal("Validation.ValidationError", error.Code); //create constants for error codes
        }
    }
}
