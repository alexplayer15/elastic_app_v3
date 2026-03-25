using Confluent.Kafka;
using System.Text.Json;
using Microsoft.Extensions.Options;
using elastic_app_v3.background_worker.Settings;

namespace elastic_app_v3.background_worker.Producer;
public class EventProducer(IOptions<KafkaSettings> kafkaSettings) : IEventProducer
{
    private readonly IProducer<Null, string> _producer = new ProducerBuilder<Null, string>(new ProducerConfig
    {
        BootstrapServers = kafkaSettings.Value.BootstrapServers,
    }).Build();
    public async Task PublishAsync(string topicName, string payload) //standardise message formats with inheritance and generics?
    {
        var kafkaMessage = new Message<Null, string>
        {
            Value = payload
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
