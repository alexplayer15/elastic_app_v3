using elastic_app_v3.domain.Entities;

namespace elastic_app_v3.application.DTOs.Subscription;
public sealed record SubscribeResponse(string Url, List<Topic> Topics);

