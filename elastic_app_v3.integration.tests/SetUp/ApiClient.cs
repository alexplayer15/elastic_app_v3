using elastic_app_v3.DTOs;
using elastic_app_v3.Errors;
using elastic_app_v3.Routing;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;

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
        public async Task<HttpResponseMessage> SendUserLoginRequest(LoginRequest loginRequest)
        {
            var uri = $"{RoutingConstants.Base}{RoutingConstants.UserLoginEndpoint}";

            return await _client.PostAsJsonAsync(uri, loginRequest);
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
        public async Task<Error?> GetErrorResponse(HttpResponseMessage response)
        {
            var contentString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(contentString))
                return null;

            return JsonSerializer.Deserialize<Error>(contentString, JsonOptions);
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }
}
