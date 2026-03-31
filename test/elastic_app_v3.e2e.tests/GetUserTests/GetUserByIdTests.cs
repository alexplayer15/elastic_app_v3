using AutoFixture;
using System.Net;
using elastic_app_v3.e2e.tests.SetUp;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.api.Errors;
using elastic_app_v3.application.DTOs.Login;
using Microsoft.AspNetCore.Mvc;
using elastic_app_v3.application.DTOs;
using elastic_app_v3.e2e.tests.InfraHelpers;

namespace elastic_app_v3.e2e.tests.GetUserTests
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
            var maxUsernameLength = 22;
            //GUID length with N is 32 chars
            var username = $"alexplayer15_{Guid.NewGuid():N}"[..maxUsernameLength];
            var password = "password";

            var user = _fixture.Build<User>()
                .With(u => u.FirstName, "Alex")
                .With(u => u.LastName, "Player")
                .With(u => u.UserName, username)
                .Without(u => u.PasswordHash)
                .Create();

            await _userDbTestHelper.AddTestUserAsync(user, password);

            var loginRequest = _fixture.Build<LoginRequest>()
                .With(u => u.UserName, username)
                .With(l => l.Password, password)
                .Create();

            var loginResponse = await _apiClient.SendUserLoginRequest(loginRequest);
            var loginResult = await _apiClient.GetResponseAsync<LoginResponse>(loginResponse);
            var token = loginResult!.AccessToken;

            //Act
            var httpResponse = await _apiClient.SendGetUserByIdRequest(token);

            //Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

            var userResponse = await _apiClient.GetResponseAsync<GetUserResponse>(httpResponse);
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
                .Without(u => u.PasswordHash)
                .Create();

            await _userDbTestHelper.AddTestUserAsync(user, password);

            var loginRequest = _fixture.Build<LoginRequest>()
                .With(u => u.UserName, username)
                .With(l => l.Password, password)
                .Create();

            var loginResponse = await _apiClient.SendUserLoginRequest(loginRequest);
            var loginResult = await _apiClient.GetResponseAsync<LoginResponse>(loginResponse);
            var token = loginResult!.AccessToken;

            await _userDbTestHelper.DeleteTestUserAsync(username);

            //Act
            var httpResponse = await _apiClient.SendGetUserByIdRequest(token);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);

            var errorResponse = await _apiClient.GetResponseAsync<ProblemDetails>(httpResponse);
            Assert.NotNull(errorResponse);
            Assert.Equal(ErrorCodes.UserDoesNotExistError, errorResponse.Type);
        }
    }
}
