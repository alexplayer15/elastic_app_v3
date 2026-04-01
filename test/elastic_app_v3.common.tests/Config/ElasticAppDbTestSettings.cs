using elastic_app_v3.infrastructure.Config;

namespace elastic_app_v3.common.tests.Config;
public static class ElasticAppDbTestSettings
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
