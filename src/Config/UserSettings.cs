namespace elastic_app_v3.Config
{
    public class UserSettings
    {
        public string Server { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool TrustServerCertificate { get; set; }
        public string GetConnectionString()
        {
            return $"Server={Server},{Port};User={User};Password={Password};" +
                   $"Database={Database};TrustServerCertificate={TrustServerCertificate};";
        }
    }
}
