using elastic_app_v3.domain.Entities;

namespace elastic_app_v3.application.DTOs.PublishMessage
{
    public sealed record PublishRequest(Topic Topic, Message Message);
}
