using elastic_app_v3.domain.Entities;
using FluentResults;

namespace elastic_app_v3.domain.Abstractions
{
    public interface ISubscriptionRepository
    {
        Task<Result<Subscription>> SubscribeAsync(Subscription subscription, CancellationToken cancellationToken);
        Task<Result<List<string>>> GetSubscriptionUrlsByTopicName(string topicName, CancellationToken cancellationToken);
    }
}
