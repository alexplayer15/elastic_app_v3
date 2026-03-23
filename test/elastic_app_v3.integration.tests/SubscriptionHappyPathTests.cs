using AutoFixture;
using System.Net;
using elastic_app_v3.application.DTOs.Subscription;
using elastic_app_v3.integration.tests.SetUp;

namespace elastic_app_v3.integration.tests
{
    [Collection(TestCollectionConstants.IntegrationTestCollectionName)]
    public class SubscriptionHappyPathTests(IntegrationTestFixture fixture)
    {
        private readonly ApiClient _apiClient = new(fixture.Client);
        private readonly Fixture _fixture = new();
        [Fact]
        public async Task GivenValidSubscriptionUrl_WhenSubscribeAsync_ThenReturnSubscribedUrl()
        {
            //Arrange
            var request = _fixture.Build<SubscribeRequest>()
                .With(r => r.Url, "https://valid-url.com/subscribe")
                .Create();

            //Act
            var response = await _apiClient.SendSubscribeRequest(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var subscribeRespone = await _apiClient.GetSubscribeResponse(response);
            Assert.NotNull(subscribeRespone);
            Assert.Equal(request.Url, subscribeRespone.Url);
        }
    }
}
