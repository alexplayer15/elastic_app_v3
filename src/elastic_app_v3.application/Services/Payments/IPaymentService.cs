using elastic_app_v3.application.DTOs.Payment;
using CSharpFunctionalExtensions;
using elastic_app_v3.domain.Errors;

namespace elastic_app_v3.application.Services.Payments
{
    public interface IPaymentService
    {
        Task<Result<PaymentResponse, PaymentError>> AddPayment(
            PaymentRequest request,
            string idempotencyKey,
            CancellationToken cancellationToken
        );
    }
}
