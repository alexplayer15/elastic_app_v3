using elastic_app_v3.infrastructure.Config;

namespace elastic_app_v3.e2e.tests.SetUp;
internal static class ElasticDbTestSettings
{
    public static ElasticDatabaseSettings SetElasticDatabaseTestSettings() => new()
    {
        Database = "Elastic",
        Server = "localhost",
        Port = 1433,
        User = "SA",
        Password = "DonutOfChocolate150!",
        TrustServerCertificate = true
    };
}
