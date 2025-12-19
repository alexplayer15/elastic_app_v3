using System.Net;
using AutoFixture;
using elastic_app_v3.Config;
using elastic_app_v3.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace elastic_app_v3.integration.tests
{
    [Collection("Integration Tests")]
    public class UserSignUpHappyPathTests
    {
        private readonly ApiClient _client;
        private readonly Fixture _fixture = new();  
        public UserSignUpHappyPathTests(CustomWebApplicationFactory<Program> factory)
        {
            var scope = factory.Services.CreateScope();
            var userSettings = scope.ServiceProvider.GetRequiredService<IOptions<UserSettings>>();

            _client = new ApiClient(factory, userSettings);

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
            var requestBody = _fixture.Create<SignUpRequest>();
            Guid? testUserId = null;

            try
            {
                var httpResponse = await _client.SendUserSignupRequest(requestBody);
                Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

                var signUpResponse = await _client.GetSignUpResponse(httpResponse);
                Assert.NotNull(signUpResponse);
                Assert.NotEqual(Guid.Empty, signUpResponse.UserId);

                testUserId = signUpResponse.UserId;
            }
            finally
            {
                if (testUserId.HasValue)
                {
                    await _client.DisposeTestDataAsync(testUserId.Value);
                }
            }
        }
    }
}
