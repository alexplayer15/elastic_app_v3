using elastic_app_v3.domain.Entities;

namespace elastic_app_v3.domain.Abstractions
{
    public interface IWebhookClient
    {
        Task SendMessage(string subscriberUrl, Message message);
    }
}
