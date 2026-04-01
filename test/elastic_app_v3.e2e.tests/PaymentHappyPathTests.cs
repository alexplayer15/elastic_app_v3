using System.Net;
using AutoFixture;
using elastic_app_v3.application.DTOs.Payment;
using elastic_app_v3.common.tests.Clients;
using elastic_app_v3.e2e.tests.SetUp;

namespace elastic_app_v3.e2e.tests
{
    [Collection(TestCollectionConstants.EndToEndTestCollectionName)]
    public class PaymentHappyPathTests(EndToEndTestFixture fixture)
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
            var paymentResponse = await _apiClient.GetResponseAsync<PaymentResponse>(response);
            Assert.NotNull(paymentResponse);
            Assert.NotEqual(Guid.Empty, paymentResponse.Id);
        }

        [Fact]
        public async Task GivenDuplicateValidPaymentRequest_WhenSendPaymentRequest_ThenReturnIdenticalPaymentResponse()
        {
            //Arrange 
            var request = _fixture.Build<PaymentRequest>()
                .With(p => p.Amount, 100)
                .With(p => p.Currency, "GBP")
                .With(p => p.Status, "ACCEPTED")
                .Create();

            var idempotencyKey = Guid.NewGuid().ToString();

            //Act
            var firstResponse = await _apiClient.SendPaymentRequest(request, idempotencyKey);

            //Assert
            Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
            var firstPaymentResponse = await _apiClient.GetResponseAsync<PaymentResponse>(firstResponse);
            Assert.NotNull(firstPaymentResponse);

            //Act 
            var secondResponse = await _apiClient.SendPaymentRequest(request, idempotencyKey);

            //Assert
            Assert.Equal(firstResponse.StatusCode, secondResponse.StatusCode);
            var secondPaymentResponse = await _apiClient.GetResponseAsync<PaymentResponse>(secondResponse);
            Assert.NotNull(secondPaymentResponse);
            Assert.Equal(firstPaymentResponse.Id, secondPaymentResponse.Id);
        }
    }
}
