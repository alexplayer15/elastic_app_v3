namespace elastic_app_v3.dashboard
{
    public class KafkaSettings
    {
        public const string KafkaSettingsName = "KafkaSettings";
        public string BootstrapServers { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
    }
}
