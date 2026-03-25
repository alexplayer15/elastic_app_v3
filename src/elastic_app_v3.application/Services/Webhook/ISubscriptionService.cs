using elastic_app_v3.application.DTOs.PublishMessage;
using elastic_app_v3.application.DTOs.Subscription;
using FluentResults;

namespace elastic_app_v3.application.Services.Webhook
{
    public interface ISubscriptionService
    {
        Task<Result<SubscribeResponse>> SubscribeAsync(SubscribeRequest request, CancellationToken cancellationToken);
    }
}
