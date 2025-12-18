using elastic_app_v3.DTOs;
using elastic_app_v3.Routing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using elastic_app_v3.Errors;

namespace elastic_app_v3.integration.tests
{
    public class ApiClient : IDisposable
    {
        private readonly HttpClient _client;

        public ApiClient(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        public async Task<HttpResponseMessage> SendUserSignupRequest(SignUpRequest request)
        {
            var uri = RoutingConstants.UserSignUpEndpoint;

            return await _client.PostAsJsonAsync(uri, request);
        }
        public async Task<SignUpResponse?> GetSignUpResponse(HttpResponseMessage response)
        {
            var contentString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(contentString))
                return null;

            return JsonSerializer.Deserialize<SignUpResponse>(
                contentString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                });
        }
        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
