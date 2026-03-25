using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Options;

namespace elastic_app_v3.dashboard.Kafka;  
public class KafkaTopicManager(IOptions<KafkaSettings> kafkaSettings) : IKafkaTopicManager
{
    private readonly string _bootstrapServers = kafkaSettings.Value.BootstrapServers;
    public async Task EnsureTopicExistsAsync(string topicName)
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = _bootstrapServers
        }).Build();

        try
        {
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
            if (metadata.Topics.Any(t => t.Topic == topicName))
            {
                Console.WriteLine($"Topic '{topicName}' already exists.");
                return;
            }

            await adminClient.CreateTopicsAsync(
            [
                new() {
                    Name = topicName,
                    NumPartitions = 1,
                    ReplicationFactor = 1
                }
            ]);

            Console.WriteLine($"Topic '{topicName}' created successfully.");
        }
        catch (CreateTopicsException e)
        {
            if (e.Results.Any(r => r.Error.Code != ErrorCode.TopicAlreadyExists))
            {
                throw; 
            }
            Console.WriteLine($"Topic '{topicName}' already exists (race condition).");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to ensure topic exists: {ex.Message}");
            throw;
        }
    }
}

