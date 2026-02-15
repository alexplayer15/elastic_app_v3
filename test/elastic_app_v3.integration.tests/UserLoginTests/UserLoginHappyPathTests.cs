using AutoFixture;
using System.Net;
using elastic_app_v3.integration.tests.SetUp;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.application.DTOs;

namespace elastic_app_v3.integration.tests.UserLoginTests
{
    [Collection(TestCollectionConstants.IntegrationTestCollectionName)]
    public class UserLoginHappyPathTests
    {
        private readonly ApiClient _apiClient;
        private readonly Fixture _fixture = new();
        private readonly UserDbTestHelper _userDbTestHelper = new();
        public UserLoginHappyPathTests(IntegrationTestFixture fixture)
        {
            _apiClient = new ApiClient(fixture.Client);
        }

        [Fact]
        public async Task GivenSignedUpUser_WhenSendUserLoginRequest_ThenReturn200AndLoginCredentials()
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
            var request = _fixture.Build<LoginRequest>()
                .With(lr => lr.UserName, username)
                .With(lr => lr.Password, password)
                .Create();

            //Act
            var response = await _apiClient.SendUserLoginRequest(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var loginResponse = await _apiClient.GetLoginResponse(response);
            Assert.NotNull(loginResponse);
            Assert.Equal(3, loginResponse.AccessToken.Split(".").Length);
            Assert.Equal(60, loginResponse.ExpiresInMinutes);
            Assert.Equal("Bearer", loginResponse.TokenType);
        }
    }
}
