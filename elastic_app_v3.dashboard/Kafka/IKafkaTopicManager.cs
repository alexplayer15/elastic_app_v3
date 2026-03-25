namespace elastic_app_v3.dashboard.Kafka
{
    public interface IKafkaTopicManager
    {
        Task EnsureTopicExistsAsync(string topicName);
    }
}
