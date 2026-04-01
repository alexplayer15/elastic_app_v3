using elastic_app_v3.integration.tests.Api;

namespace elastic_app_v3.integration.tests.Fixtures;
public class ApiFixture : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    public HttpClient Client { get; }
    public ApiFixture()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        Client = _factory.CreateClient();
    }
    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
    public Task DisposeAsync()
    {
        Client.Dispose();
        _factory.Dispose();
        return Task.CompletedTask;
    }
}

[CollectionDefinition(TestCollectionConstants.ApiTestCollectionName)]
public class ApiTestCollection :
  ICollectionFixture<ApiFixture>
{
}