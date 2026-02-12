using System.Net;
using AutoFixture;
using elastic_app_v3.Enums;
using elastic_app_v3.Domain;
using elastic_app_v3.DTOs;
using elastic_app_v3.integration.tests.SetUp;

namespace elastic_app_v3.integration.tests.UserLoginTests
{
    [Collection(TestCollectionConstants.IntegrationTestCollectionName)]
    public class UserLoginUnhappyPathTests
    {
        private readonly ApiClient _apiClient;
        private readonly Fixture _fixture = new();
        private readonly UserDbTestHelper _userDbTestHelper = new();
        public UserLoginUnhappyPathTests(IntegrationTestFixture fixture)
        {
            _apiClient = new ApiClient(fixture.Client);

            var maxUsernameLength = 22;
            var username = $"alexplayer15_{Guid.NewGuid():N}"[..maxUsernameLength];

            _fixture.Customize<LoginRequest>(lr => lr
                .With(lr => lr.UserName, username)
                .With(lr => lr.Password, "password")
            );
        }

        [Fact]
        public async Task GivenNonSignedUpUser_WhenSendUserLoginRequest_ThenReturn404AndError()
        {
            //Arrange
            var request = _fixture.Create<LoginRequest>();

            //Act
            var response = await _apiClient.SendUserLoginRequest(request);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var errorResponse = await _apiClient.GetErrorResponse(response);
            Assert.NotNull(errorResponse);
            Assert.Equal(ErrorCategory.UserDoesNotExist, errorResponse.ErrorCategory);
        }

        [Fact]
        public async Task GivenSignedUpUserEnteringWrongPassword_WhenSendUserLoginRequest_ThenReturnUnauthorized()
        {
            //Arrange
            var request = _fixture.Create<LoginRequest>() with
            {
                Password = "passwoooooord",
            };

            var user = _fixture.Build<User>()
                .With(u => u.FirstName, "Alex")
                .With(u => u.LastName, "Player")
                .With(u => u.UserName, request.UserName)
                .Without(u => u.PasswordHash)
                .Create();

            await _userDbTestHelper.AddTestUserAsync(user, "password");

            //Act
            var response = await _apiClient.SendUserLoginRequest(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            //var errorResponse = await _apiClient.GetErrorResponse(response);
            //Assert.NotNull(errorResponse);
        }
    }
}
