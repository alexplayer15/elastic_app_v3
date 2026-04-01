using elastic_app_v3.infrastructure.Config;
using elastic_app_v3.common.tests.Config;

namespace elastic_app_v3.integration.tests.Fixtures;
public class ElasticAppDbFixture
{
    public ElasticDatabaseSettings ElasticDatabaseSettings { get; private set; }
    public ElasticAppDbFixture()
    {
        ElasticDatabaseSettings = ElasticAppDbTestSettings.SetElasticDatabaseTestSettings();
    }
}

[CollectionDefinition(TestCollectionConstants.ElasticAppDbTestCollectionName, DisableParallelization = true)]
public class ElasticAppDbTestCollection :
    ICollectionFixture<ElasticAppDbFixture>
{
}

