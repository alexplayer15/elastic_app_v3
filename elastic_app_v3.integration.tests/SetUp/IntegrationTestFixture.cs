using elastic_app_v3.Config;
using Microsoft.Data.SqlClient;
using Dapper;

namespace elastic_app_v3.integration.tests.SetUp
{
    public class IntegrationTestFixture : IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        public HttpClient Client { get; }
        public UserSettings UserSettings { get; private set; }
        private readonly string _connectionString;
        public IntegrationTestFixture()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            Client = _factory.CreateClient();
            UserSettings = SetUserSetting();
            _connectionString = UserSettings.GetConnectionString();
        }
        public async Task InitializeAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sqlQuery = "DELETE FROM Users;";

                await connection.ExecuteAsync(sqlQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while clearing up the test data", ex);
            }
        }
        private static UserSettings SetUserSetting() => new()
        {
            Database = "Users",
            Server = "localhost",
            Port = 1433,
            User = "SA",
            Password = "DonutOfChocolate150!",
            TrustServerCertificate = true
        };
        public Task DisposeAsync()
        {
            Client.Dispose();
            _factory.Dispose();
            return Task.CompletedTask;
        }
    }

    [CollectionDefinition(TestCollectionConstants.IntegrationTestCollectionName, DisableParallelization = true)]
    public class IntegrationTestCollection :
    ICollectionFixture<IntegrationTestFixture>
    {
    }
}
