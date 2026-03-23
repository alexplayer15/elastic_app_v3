using elastic_app_v3.application.DTOs.Payment;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Entities;
using FluentResults;

namespace elastic_app_v3.application.Services.Payments
{
    public class PaymentService(IPaymentRepository paymentRepository) : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository = paymentRepository;
        public async Task<Result<PaymentResponse>> AddPayment(
            PaymentRequest request,
            string idempotencyKey,
            CancellationToken cancellationToken
        )
        {
            //to do: payment request validations

            var existingPayment = await _paymentRepository.CheckIfIdempotencyKeyExists(idempotencyKey, cancellationToken);

            if(existingPayment.Value != Guid.Empty)
            {
                return existingPayment.Map(id => new PaymentResponse(id));
            }

            var payment = new Payment()
            {
                Amount = request.Amount, 
                Status = request.Status,
                Currency = request.Currency
            };

            var result = await _paymentRepository.AddPaymentAsync(
                payment, 
                idempotencyKey,
                cancellationToken
            );

            return result.Map(id => new PaymentResponse(id));
        }
    }
}
