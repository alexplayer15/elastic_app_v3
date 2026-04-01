using elastic_app_v3.infrastructure.Config;

namespace elastic_app_v3.integration.tests.Fixtures;
public class ElasticAppDbFixture
{
    public ElasticDatabaseSettings ElasticDatabaseSettings { get; private set; }
    public ElasticAppDbFixture()
    {
        ElasticDatabaseSettings = SetElasticDatabaseTestSettings();
    }
    private static ElasticDatabaseSettings SetElasticDatabaseTestSettings() => new()
    {
        Database = "Elastic",
        Server = "localhost",
        Port = 1433,
        User = "SA",
        Password = "DonutOfChocolate150!",
        TrustServerCertificate = true
    };
}

[CollectionDefinition(TestCollectionConstants.ElasticAppDbTestCollectionName, DisableParallelization = true)]
public class ElasticAppDbTestCollection :
    ICollectionFixture<ElasticAppDbFixture>
{
}

