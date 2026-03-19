using elastic_app_v3.application.DTOs;
using FluentResults;

namespace elastic_app_v3.application.Services
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
