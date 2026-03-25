using Confluent.Kafka;
using elastic_app_v3.dashboard.Kafka;
using Microsoft.Extensions.Options;

namespace elastic_app_v3.dashboard.Consumer;
public class EventConsumer : BackgroundService
{
    private readonly IOptions<KafkaSettings> _kafkaSettings;
    private readonly IKafkaTopicManager _kafkaTopicManager;
    private readonly IConsumer<Ignore, string> _consumer;
    public EventConsumer(
        IOptions<KafkaSettings> kafkaSettings,
        IKafkaTopicManager kafkaTopicManager
    ) 
    {
        _kafkaSettings = kafkaSettings;
        _kafkaTopicManager = kafkaTopicManager;

        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.Value.BootstrapServers,
            GroupId = _kafkaSettings.Value.GroupId,      
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
    }
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _kafkaTopicManager.EnsureTopicExistsAsync("UserSignedUp");
        _consumer.Subscribe("UserSignedUp");
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                if (consumeResult != null)
                {
                    Console.WriteLine($"Received message: {consumeResult.Message.Value}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        finally
        {
            _consumer.Close();
        };
    }
}