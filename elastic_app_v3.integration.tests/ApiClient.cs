using elastic_app_v3.DTOs;
using elastic_app_v3.Routing;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using elastic_app_v3.Config;
using Microsoft.Data.SqlClient;
using Dapper;

namespace elastic_app_v3.integration.tests
{
    public class ApiClient(
        CustomWebApplicationFactory<Program> factory,
        IOptions<UserSettings> userSettings) : IDisposable
    {
        private readonly HttpClient _client = factory.CreateClient();
        private readonly string _connectionString = userSettings.Value.GetConnectionString();
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
        public async Task DisposeTestDataAsync(Guid testUserId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sqlQuery = "DELETE FROM Users WHERE Id = @Id;";

                await connection.ExecuteAsync(sqlQuery, new { Id = testUserId });
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while clearing up the test data", ex);
            }
        }
        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
