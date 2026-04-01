using AutoFixture;
using System.Net;
using elastic_app_v3.e2e.tests.SetUp;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.api.Errors;
using Microsoft.AspNetCore.Mvc;
using elastic_app_v3.application.DTOs;
using elastic_app_v3.common.tests.Clients;
using elastic_app_v3.common.tests;

namespace elastic_app_v3.e2e.tests.GetUserTests
{
    [Collection(TestCollectionConstants.EndToEndTestCollectionName)]
    public class GetUserByIdTests(EndToEndTestFixture fixture)
    {
        private readonly ApiClient _apiClient = new(fixture.Client);
        private readonly ElasticAppDbClient _userDbTestHelper = new();
        private readonly Fixture _fixture = new();

        [Fact]
        public async Task GivenExistingUserId_WhenSendGetUserById_ThenReturn200AndUser()
        {
            //Arrange
            var maxUsernameLength = 22;
            //GUID length with N is 32 chars
            var username = $"alexplayer15_{Guid.NewGuid():N}"[..maxUsernameLength];

            var user = _fixture.Build<User>()
                .With(u => u.FirstName, "Alex")
                .With(u => u.LastName, "Player")
                .With(u => u.UserName, username)
                .Without(u => u.PasswordHash)
                .Create();

            var userId = await _userDbTestHelper.AddTestUserAsync(user);

            var token = TokenHelper.GenerateTestToken(userId);

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

            var user = _fixture.Build<User>()
                .With(u => u.FirstName, "Alex")
                .With(u => u.LastName, "Player")
                .With(u => u.UserName, username)
                .Without(u => u.PasswordHash)
                .Create();

            var userId = await _userDbTestHelper.AddTestUserAsync(user);

            await _userDbTestHelper.DeleteTestUserAsync(username);

            var token = TokenHelper.GenerateTestToken(userId);

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
