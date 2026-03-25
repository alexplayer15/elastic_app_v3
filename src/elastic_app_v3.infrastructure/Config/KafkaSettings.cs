namespace elastic_app_v3.infrastructure.Config
{
    public class KafkaSettings
    {
        public const string KafkaSettingsName = "KafkaSettings";
        public string BootstrapServers { get; set; } = string.Empty;
    }
}
