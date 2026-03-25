namespace elastic_app_v3.background_worker.Producer
{
    public interface IEventProducer
    {
        Task PublishAsync(string topicName, string payload);
    }
}
