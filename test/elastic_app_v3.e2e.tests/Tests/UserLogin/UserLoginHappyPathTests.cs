using System.Net;
using AutoFixture;
using elastic_app_v3.application.DTOs.Login;
using elastic_app_v3.common.tests.Clients;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.e2e.tests.Constants;
using elastic_app_v3.e2e.tests.Fixtures;

namespace elastic_app_v3.e2e.tests.Tests.UserLogin
{
    [Collection(TestCollectionConstants.EndToEndTestCollectionName)]
    public class UserLoginHappyPathTests(EndToEndTestFixture fixture)
    {
        private readonly ApiClient _apiClient = new(fixture.Client);
        private readonly Fixture _fixture = new();
        private readonly ElasticAppDbClient _userDbTestHelper = new();

        [Fact]
        public async Task GivenSignedUpUser_WhenSendUserLoginRequest_ThenReturn204AndPopulateHttpContext()
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
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    
            var accessTokenCookie = response.Headers
                .Where(h => h.Key == "Set-Cookie")
                .SelectMany(h => h.Value)
                .FirstOrDefault(c => c.StartsWith("accessToken="));

            Assert.NotNull(accessTokenCookie);
            Assert.Contains("httponly", accessTokenCookie, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("samesite=strict", accessTokenCookie, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("secure", accessTokenCookie, StringComparison.OrdinalIgnoreCase);
            
            var tokenValue = accessTokenCookie.Split(';')[0].Split('=')[1];
            Assert.Equal(3, tokenValue.Split('.').Length);
        }
    }
}
