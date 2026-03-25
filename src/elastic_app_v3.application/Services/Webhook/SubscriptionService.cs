using FluentResults;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.application.DTOs.Subscription;

namespace elastic_app_v3.application.Services.Webhook
{
    public class SubscriptionService(ISubscriptionRepository subscriptionRepository) : ISubscriptionService
    { 
        private readonly ISubscriptionRepository _subscriptionRepository = subscriptionRepository;
        public async Task<Result<SubscribeResponse>> SubscribeAsync(SubscribeRequest request, CancellationToken cancellationToken)
        {
            //to do: validate url

            var subscription = new Subscription
            {
                Url = request.Url,
                Topics = request.Topics,
            };

            var result = await _subscriptionRepository.SubscribeAsync(subscription, cancellationToken);

            return result.Map(subscription => new SubscribeResponse(subscription.Url, subscription.Topics));
        }
    }
}
