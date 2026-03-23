using elastic_app_v3.api.Routing;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;
using elastic_app_v3.application.DTOs;
using elastic_app_v3.api.Errors;
using elastic_app_v3.application.DTOs.Payment;
using elastic_app_v3.application.DTOs.Login;
using elastic_app_v3.application.DTOs.SingUp;
using elastic_app_v3.application.DTOs.Subscription;

namespace elastic_app_v3.integration.tests.SetUp
{
    public class ApiClient(HttpClient client)
    {
        private readonly HttpClient _client = client;
        public async Task<HttpResponseMessage> SendUserSignupRequest(SignUpRequest request)
        {
            var uri = $"{RoutingConstants.Base}{RoutingConstants.UserSignUpEndpoint}";

            return await _client.PostAsJsonAsync(uri, request);
        }
        public async Task<HttpResponseMessage> SendGetUserByIdRequest(string token)
        {
            var uri = $"{RoutingConstants.Base}{RoutingConstants.GetUserByIdEndpoint}";
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return await _client.SendAsync(request);
        }
        public async Task<HttpResponseMessage> SendUserLoginRequest(LoginRequest request)
        {
            var uri = $"{RoutingConstants.Base}{RoutingConstants.UserLoginEndpoint}";

            return await _client.PostAsJsonAsync(uri, request);
        }
        public async Task<HttpResponseMessage> SendPaymentRequest(
            PaymentRequest request,
            string idempotencyKey)
        {
            var uri = $"{RoutingConstants.Base}{RoutingConstants.PaymentEndpoint}";

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(request)
            };

            httpRequest.Headers.Add("Idempotency-Key", idempotencyKey);

            return await _client.SendAsync(httpRequest);
        }
        public async Task<HttpResponseMessage> SendSubscribeRequest(SubscribeRequest request)
        {
            var uri = $"{RoutingConstants.Base}{RoutingConstants.SubscribeEndpoint}";

            return await _client.PostAsJsonAsync(uri, request);
        }
        public async Task<SignUpResponse?> GetSignUpResponse(HttpResponseMessage response)
        {
            var contentString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(contentString))
                return null;

            return JsonSerializer.Deserialize<SignUpResponse>(contentString, JsonOptions);
        }
        public async Task<GetUserResponse?> GetUserResponse(HttpResponseMessage response)
        {
            var contentString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(contentString))
                return null;

            return JsonSerializer.Deserialize<GetUserResponse>(contentString, JsonOptions);      
        }
        public async Task<LoginResponse?> GetLoginResponse(HttpResponseMessage response)
        {
            var contentString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(contentString))
                return null;

            return JsonSerializer.Deserialize<LoginResponse>(contentString, JsonOptions);
        }
        public async Task<ApiError?> GetErrorResponse(HttpResponseMessage response)
        {
            var contentString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(contentString))
                return null;

            return JsonSerializer.Deserialize<ApiError>(contentString, JsonOptions);
        }
        public async Task<PaymentResponse?> GetPaymentResponse(HttpResponseMessage response)
        {
            var contentString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(contentString))
                return null;

            return JsonSerializer.Deserialize<PaymentResponse>(contentString, JsonOptions);
        }
        public async Task<SubscribeResponse?> GetSubscribeResponse(HttpResponseMessage response)
        {
            var contentString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(contentString))
                return null;

            return JsonSerializer.Deserialize<SubscribeResponse>(contentString, JsonOptions);
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }
}
