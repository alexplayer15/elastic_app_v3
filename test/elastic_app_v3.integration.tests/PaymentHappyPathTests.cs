using elastic_app_v3.integration.tests.SetUp;
using AutoFixture;
using System.Net;
using elastic_app_v3.application.DTOs;

namespace elastic_app_v3.integration.tests
{
    [Collection(TestCollectionConstants.IntegrationTestCollectionName)]
    public class PaymentHappyPathTests(IntegrationTestFixture fixture)
    {
        private readonly ApiClient _apiClient = new(fixture.Client);
        private readonly Fixture _fixture = new();

        [Fact]
        public async Task GivenValidPayment_WhenSendPaymentRequest_ThenReturnSuccessfulPaymentResponse()
        {
            //Arrange 
            var request = _fixture.Build<PaymentRequest>()
                .With(p => p.Amount, 100)
                .With(p => p.Currency, "GBP")
                .With(p => p.Status, "ACCEPTED")
                .Create();

            //Act
            var response = await _apiClient.SendPaymentRequest(request, Guid.NewGuid().ToString());

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var paymentResponse = await _apiClient.GetPaymentResponse(response);
            Assert.NotNull(paymentResponse);
            Assert.NotEqual(Guid.Empty, paymentResponse.Id);
        }
    }
}
