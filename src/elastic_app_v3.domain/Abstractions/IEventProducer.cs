using elastic_app_v3.domain.Events;

namespace elastic_app_v3.domain.Abstractions
{
    public interface IEventProducer
    {
        Task PublishAsync<T>(string topicName, T message) where T : Event;
    }
}
