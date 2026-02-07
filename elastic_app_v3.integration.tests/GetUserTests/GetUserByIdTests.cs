using AutoFixture;
using System.Net;
using elastic_app_v3.Domain;
using elastic_app_v3.integration.tests.SetUp;
using elastic_app_v3.Enums;

namespace elastic_app_v3.integration.tests.GetUserTests
{
    [Collection(TestCollectionConstants.IntegrationTestCollectionName)]
    public class GetUserByIdTests
    {
        private readonly ApiClient _apiClient;
        private readonly UserDbTestHelper _userDbTestHelper = new();
        private readonly Fixture _fixture = new();

        public GetUserByIdTests(IntegrationTestFixture fixture)
        {
            _apiClient = new ApiClient(fixture.Client);
        }

        [Fact]
        public async Task GivenExistingUserId_WhenSendGetUserById_ThenReturn200AndUser()
        {
            //Arrange
            var username = "alexplayer15";

            var user = _fixture.Build<User>()
                .With(u => u.FirstName, "Alex")
                .With(u => u.LastName, "Player")
                .With(u => u.UserName, username)
                .With(u => u.PasswordHash, "password")
                .Create();

            await _userDbTestHelper.AddTestUserAsync(user);

            var userId = await _userDbTestHelper.GetUserIdAsync(username);

            //Act
            var httpResponse = await _apiClient.SendGetUserByIdRequest(userId);

            //Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

            var userResponse = await _apiClient.GetUserResponse(httpResponse);
            Assert.NotNull(userResponse);
            Assert.Equal(username, userResponse.UserName);
        }

        [Fact]
        public async Task GivenNonExistingUserId_WhenSendGetUserById_ThenReturn404AndError()
        {
            //Act
            var httpResponse = await _apiClient.SendGetUserByIdRequest(new Guid());

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);

            var errorResponse = await _apiClient.GetErrorResponse(httpResponse);
            Assert.NotNull(errorResponse);
            Assert.Equal(ErrorCategory.UserDoesNotExist, errorResponse.ErrorCategory);
        }
    }
}
