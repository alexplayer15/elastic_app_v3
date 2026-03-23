using FluentResults;

namespace elastic_app_v3.domain.Abstractions
{
    public interface ISubscriptionRepository
    {
        Task<Result<string>> SubscribeAsync(string url, CancellationToken cancellationToken);
    }
}
