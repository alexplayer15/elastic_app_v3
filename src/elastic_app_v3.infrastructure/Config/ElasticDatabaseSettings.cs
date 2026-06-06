namespace elastic_app_v3.infrastructure.Config
{
    public class ElasticDatabaseSettings
    {
        public const string ElasticDatabaseSettingsName = "ElasticDatabaseSettings";
        public string Server { get; init; } = string.Empty;
        public string User { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string Database { get; init; } = string.Empty;
        public int Port { get; init; }
        public bool TrustServerCertificate { get; init; }
        public string GetConnectionString()
        {
            return $"Server={Server},{Port};User={User};Password={Password};" +
                   $"Database={Database};TrustServerCertificate={TrustServerCertificate};";
        }
    }
}
