using elastic_app_v3.domain.Entities;
using CSharpFunctionalExtensions;
using elastic_app_v3.domain.Errors;

namespace elastic_app_v3.domain.Abstractions
{
    public interface IPaymentRepository
    {
        Task<Result<Guid, PaymentError>> AddPaymentAsync(
            Payment payment, 
            string idempotencyKey, 
            CancellationToken cancellationToken
        );
        
        Task<Result<Guid, PaymentError>> CheckIfIdempotencyKeyExists(
            string idempotencyKey, 
            CancellationToken cancellation
        );
    }
}
