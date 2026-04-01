using elastic_app_v3.api.Routing.Constants;
using elastic_app_v3.application.DTOs.Login;
using elastic_app_v3.application.DTOs.SignUp;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace elastic_app_v3.integration.tests.Clients;
public class ApiClient(HttpClient client)
{
    private readonly HttpClient _client = client;
    public async Task<HttpResponseMessage> SendUserSignupRequest(SignUpRequest request)
    {
        var uri = $"{EndpointConstants.Base}{EndpointConstants.UserSignUpEndpoint}";

        return await _client.PostAsJsonAsync(uri, request);
    }

    //merge this and sign up method into reusable one?
    public async Task<HttpResponseMessage> SendUserLoginRequest(LoginRequest request)
    {
        var uri = $"{EndpointConstants.Base}{EndpointConstants.UserLoginEndpoint}";

        return await _client.PostAsJsonAsync(uri, request);
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
