using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Entities;

namespace elastic_app_v3.infrastructure.Clients
{
    public class WebhookClient(HttpClient httpClient) : IWebhookClient
    {
        private readonly HttpClient _httpClient = httpClient;
        public async Task SendMessage(string subscriberUrl, Message message)
        {
            await _httpClient.PostAsJsonAsync(subscriberUrl, message);
        }
    }
}
