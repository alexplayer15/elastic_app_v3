using Microsoft.Data.SqlClient;
using Dapper;
using elastic_app_v3.infrastructure.Config;

namespace elastic_app_v3.e2e.tests.SetUp
{
    public class IntegrationTestFixture : IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        public HttpClient Client { get; }
        public ElasticDatabaseSettings ElasticDatabaseSettings { get; private set; }
        private readonly string _connectionString;
        public IntegrationTestFixture()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            Client = _factory.CreateClient();
            ElasticDatabaseSettings = ElasticDbTestSettings.SetElasticDatabaseTestSettings();
            _connectionString = ElasticDatabaseSettings.GetConnectionString();
        }
        public async Task InitializeAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sqlQuery = @"
                DELETE FROM Languages;
                DELETE FROM Profiles;
                DELETE FROM Users;
                ";

                await connection.ExecuteAsync(sqlQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while clearing up the test data", ex);
            }
        }
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
