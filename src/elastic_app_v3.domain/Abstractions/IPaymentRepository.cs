using elastic_app_v3.domain.Entities;
using FluentResults;

namespace elastic_app_v3.domain.Abstractions
{
    public interface IPaymentRepository
    {
        Task<Result<Guid>> AddPaymentAsync(Payment payment, string idempotencyKey, CancellationToken cancellationToken);

        Task<Result<Guid>> CheckIfIdempotencyKeyExists(string idempotencyKey, CancellationToken cancellation);
    }
}
