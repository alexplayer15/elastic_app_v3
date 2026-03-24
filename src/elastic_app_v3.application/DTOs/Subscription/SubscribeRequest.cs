using elastic_app_v3.domain.Entities;

namespace elastic_app_v3.application.DTOs.Subscription
{
    public sealed record SubscribeRequest(string Url, List<Topic> Topics);
}
