using System.Net;
using AutoFixture;
using elastic_app_v3.DTOs;

namespace elastic_app_v3.integration.tests
{
    [Collection("Integration Tests")]
    public class UserSignUpHappyPathTests
    {
        private readonly ApiClient _client;
        private readonly Fixture _fixture = new();
        public UserSignUpHappyPathTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = new ApiClient(factory);

            _fixture = new Fixture();

            _fixture.Customize<SignUpRequest>(c => c
                .With(x => x.FirstName, "Alex")
                .With(x => x.LastName, "Player")
                .With(x => x.UserName, "alexplayer15")
                .With(x => x.Password, "purple")
                .With(x => x.ReEnteredPassword, "purple")
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
