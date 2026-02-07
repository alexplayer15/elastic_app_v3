using System.Net;
using AutoFixture;
using elastic_app_v3.DTOs;
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

            _fixture.Customize<SignUpRequest>(c => c
                .With(x => x.FirstName, "Alex")
                .With(x => x.LastName, "Player")
                .With(x => x.UserName, "alexplayer15")
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
