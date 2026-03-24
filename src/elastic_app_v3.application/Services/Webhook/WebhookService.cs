using FluentResults;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.application.DTOs.Subscription;
using elastic_app_v3.application.DTOs.PublishMessage;

namespace elastic_app_v3.application.Services.Webhook
{
    public class WebhookService(
        ISubscriptionRepository subscriptionRepository,
        IWebhookClient webhookClient) : IWebhookService
    { 
        private readonly ISubscriptionRepository _subscriptionRepository = subscriptionRepository;
        private readonly IWebhookClient _webhookClient = webhookClient;
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
        public async Task<Result> PublishAsync(PublishRequest request, CancellationToken cancellationToken)
        {
            var subscriptionUrls = await _subscriptionRepository.GetSubscriptionUrlsByTopicName(request.Topic.Name, cancellationToken);

            if (subscriptionUrls.Value.Count == 0)
            {
                return Result.Fail("No existing subscriptions for this topic");
            }

            try
            {
                await Task.WhenAll(
                    subscriptionUrls.Value.Select(url =>
                        _webhookClient.SendMessage(url, request.Message)
                    )
                );

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Failed to publish message: {ex.Message}");
            } //how would this fail if one fails in a batch?
        }
    }
}
