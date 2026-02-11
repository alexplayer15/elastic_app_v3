using AutoFixture;
using System.Net;
using elastic_app_v3.Domain;
using elastic_app_v3.integration.tests.SetUp;
using elastic_app_v3.Enums;
using elastic_app_v3.DTOs;

namespace elastic_app_v3.integration.tests.GetUserTests
{
    [Collection(TestCollectionConstants.IntegrationTestCollectionName)]
    public class GetUserByIdTests(IntegrationTestFixture fixture)
    {
        private readonly ApiClient _apiClient = new(fixture.Client);
        private readonly UserDbTestHelper _userDbTestHelper = new();
        private readonly Fixture _fixture = new();

        [Fact]
        public async Task GivenExistingUserId_WhenSendGetUserById_ThenReturn200AndUser()
        {
            //Arrange
            var username = "alexplayer15";
            var password = "password";

            var user = _fixture.Build<User>()
                .With(u => u.FirstName, "Alex")
                .With(u => u.LastName, "Player")
                .With(u => u.UserName, username)
                .With(u => u.PasswordHash, password)
                .Create();

            await _userDbTestHelper.AddTestUserAsync(user);

            var loginRequest = _fixture.Build<LoginRequest>()
                .With(u => u.UserName, username)
                .With(l => l.Password, password)
                .Create();

            var loginResponse = await _apiClient.SendUserLoginRequest(loginRequest);
            var loginResult = await _apiClient.GetLoginResponse(loginResponse);
            var token = loginResult!.AccessToken;

            //Act
            var httpResponse = await _apiClient.SendGetUserByIdRequest(token);

            //Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

            var userResponse = await _apiClient.GetUserResponse(httpResponse);
            Assert.NotNull(userResponse);
            Assert.Equal(username, userResponse.UserName);
        }

        [Fact]
        public async Task GivenNonExistingUserId_WhenSendGetUserById_ThenReturn404AndError()
        {
            //Arrange
            var username = "alexplayer15";
            var password = "password";

            var user = _fixture.Build<User>()
                .With(u => u.FirstName, "Alex")
                .With(u => u.LastName, "Player")
                .With(u => u.UserName, username)
                .With(u => u.PasswordHash, password)
                .Create();

            await _userDbTestHelper.AddTestUserAsync(user);

            var loginRequest = _fixture.Build<LoginRequest>()
                .With(u => u.UserName, username)
                .With(l => l.Password, password)
                .Create();

            var loginResponse = await _apiClient.SendUserLoginRequest(loginRequest);
            var loginResult = await _apiClient.GetLoginResponse(loginResponse);
            var token = loginResult!.AccessToken;

            await _userDbTestHelper.DeleteTestUserAsync(username);

            //Act
            var httpResponse = await _apiClient.SendGetUserByIdRequest(token);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);

            var errorResponse = await _apiClient.GetErrorResponse(httpResponse);
            Assert.NotNull(errorResponse);
            Assert.Equal(ErrorCategory.UserDoesNotExist, errorResponse.ErrorCategory);
        }
    }
}
