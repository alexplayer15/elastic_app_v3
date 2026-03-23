using FluentResults;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.application.DTOs.Subscription;

namespace elastic_app_v3.application.Services.Webhook
{
    public class WebhookService(ISubscriptionRepository subscriptionRepository) : IWebhookService
    { 
        private readonly ISubscriptionRepository _subscriptionRepository = subscriptionRepository;
        public async Task<Result<SubscribeResponse>> SubscribeAsync(SubscribeRequest request, CancellationToken cancellationToken)
        {
            //to do: validate url

            var result = await _subscriptionRepository.SubscribeAsync(request.Url, cancellationToken);

            return result.Map(url => new SubscribeResponse(url));
        }
    }
}
