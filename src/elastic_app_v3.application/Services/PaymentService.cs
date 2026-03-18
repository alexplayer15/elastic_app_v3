using elastic_app_v3.application.DTOs;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Entities;
using FluentResults;

namespace elastic_app_v3.application.Services
{
    public class PaymentService(IPaymentRepository paymentRepository) : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository = paymentRepository;
        public async Task<Result<PaymentResponse>> AddPayment(
            PaymentRequest request,
            CancellationToken cancellationToken)
        {
            var payment = new Payment()
            {
                Amount = request.Amount, 
                Status = request.Status,
                Currency = request.Currency
            };

            var result = await _paymentRepository.AddPayment(payment, cancellationToken);

            return result.Map(id => new PaymentResponse(id));
        }
    }
}
