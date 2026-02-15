using System.Net;
using AutoFixture;
using elastic_app_v3.application.DTOs;
using elastic_app_v3.integration.tests.SetUp;

namespace elastic_app_v3.integration.tests.UserSignUpTests
{
    [Collection(TestCollectionConstants.IntegrationTestCollectionName)]
    public class UserSignUpHappyPathTests
    {
        private readonly ApiClient _client;
        private readonly Fixture _fixture = new();
        public UserSignUpHappyPathTests(IntegrationTestFixture fixture)
        {
            _client = new ApiClient(fixture.Client);

            var maxUsernameLength = 22;
            //GUID length with N is 32 chars
            var username = $"alexplayer15_{Guid.NewGuid():N}"[..maxUsernameLength];

            _fixture.Customize<SignUpRequest>(c => c
                .With(x => x.FirstName, "Alex")
                .With(x => x.LastName, "Player")
                .With(x => x.UserName, username)
                .With(x => x.Password, "password") //what if validation requirements change?
                .With(x => x.ReEnteredPassword, "password")
            );
        }

        [Fact]
        public async Task GivenValidUserSignUpRequest_WhenSendUserSignUpRequest_ThenReturn200AndUserId()
        {
            //Arrange
            var requestBody = _fixture.Create<SignUpRequest>();

            //Act
            var httpResponse = await _client.SendUserSignupRequest(requestBody);

            //Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

            var signUpResponse = await _client.GetSignUpResponse(httpResponse);
            Assert.NotNull(signUpResponse);
            Assert.NotEqual(Guid.Empty, signUpResponse.UserId);
        }
    }
}
