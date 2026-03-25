using Confluent.Kafka;
using elastic_app_v3.domain.Events;
using elastic_app_v3.infrastructure.Config;
using System.Text.Json;
using Microsoft.Extensions.Options;
using elastic_app_v3.domain.Abstractions;

namespace elastic_app_v3.infrastructure.Events
{
    public class EventProducer(IOptions<KafkaSettings> kafkaSettings) : IEventProducer
    {
        private readonly IProducer<Null, string> _producer = new ProducerBuilder<Null, string>(new ProducerConfig
        {
            BootstrapServers = kafkaSettings.Value.BootstrapServers,
        }).Build();
        public async Task PublishAsync<T>(string topicName, T message) where T : Event
        {

            var kafkaMessage = new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(message)
            };
            try
            {
               await _producer.ProduceAsync(topicName, kafkaMessage);
            }
            catch (Exception)
            {
                throw;

            }

        }
    }
}
