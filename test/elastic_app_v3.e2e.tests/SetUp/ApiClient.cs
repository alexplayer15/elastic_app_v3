using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;
using elastic_app_v3.application.DTOs.Payment;
using elastic_app_v3.application.DTOs.Login;
using elastic_app_v3.api.Routing.Constants;
using elastic_app_v3.application.DTOs.SignUp;
using elastic_app_v3.application.DTOs.Profile;

namespace elastic_app_v3.e2e.tests.SetUp
{
    public class ApiClient(HttpClient client)
    {
        private readonly HttpClient _client = client;
        public async Task<HttpResponseMessage> SendUserSignupRequest(SignUpRequest request)
        {
            var uri = $"{EndpointConstants.Base}{EndpointConstants.UserSignUpEndpoint}";

            return await _client.PostAsJsonAsync(uri, request);
        }
        public async Task<HttpResponseMessage> SendGetUserByIdRequest(string token)
        {
            var uri = $"{EndpointConstants.Base}{EndpointConstants.GetUserByIdEndpoint}";
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return await _client.SendAsync(request);
        }
        public async Task<HttpResponseMessage> SendUserLoginRequest(LoginRequest request)
        {
            var uri = $"{EndpointConstants.Base}{EndpointConstants.UserLoginEndpoint}";

            return await _client.PostAsJsonAsync(uri, request);
        }
        public async Task<HttpResponseMessage> SendPaymentRequest(
            PaymentRequest request,
            string idempotencyKey)
        {
            var uri = $"{EndpointConstants.Base}{EndpointConstants.PaymentEndpoint}";

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(request)
            };

            httpRequest.Headers.Add("Idempotency-Key", idempotencyKey);

            return await _client.SendAsync(httpRequest);
        }
        public async Task<HttpResponseMessage> SendUpdateProfileRequest(
           UpdateProfileRequest request,
           string token)
        {
            var uri = $"{EndpointConstants.Base}{EndpointConstants.UpdateProfileEndpoint}";

            var httpRequest = new HttpRequestMessage(HttpMethod.Patch, uri)
            {
                Content = JsonContent.Create(request)
            };

            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return await _client.SendAsync(httpRequest);
        }
        public async Task<T?> GetResponseAsync<T>(HttpResponseMessage response) where T : class
        {
            var contentString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(contentString))
                return null;

            return JsonSerializer.Deserialize<T>(contentString, JsonOptions);
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }
}
