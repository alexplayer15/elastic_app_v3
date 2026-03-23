using elastic_app_v3.application.DTOs.Payment;
using FluentResults;

namespace elastic_app_v3.application.Services.Payments
{
    public interface IPaymentService
    {
        Task<Result<PaymentResponse>> AddPayment(
            PaymentRequest request,
            string idempotencyKey,
            CancellationToken cancellationToken
        );
    }
}
