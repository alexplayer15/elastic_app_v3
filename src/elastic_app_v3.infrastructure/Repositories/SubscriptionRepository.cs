using Dapper;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.infrastructure.Config;
using elastic_app_v3.infrastructure.Constants;
using elastic_app_v3.infrastructure.Models;
using FluentResults;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Registry;

namespace elastic_app_v3.infrastructure.Repositories
{
    public class SubscriptionRepository(
        IOptions<ElasticDatabaseSettings> elasticDatabaseSettings,
        ResiliencePipelineProvider<string> resiliencePipelineProvider) : ISubscriptionRepository
    {
        private readonly string _connectionString = elasticDatabaseSettings.Value.GetConnectionString();
        private readonly ResiliencePipeline _resiliencePipeline
            = resiliencePipelineProvider.GetPipeline(ResiliencePolicy.UserResiliencePolicyKey); //worth having different policies for diffrent repositories?
        public async Task<Result<Subscription>> SubscribeAsync(Subscription subscription, CancellationToken cancellationToken)
        {
            try
            {
                return await _resiliencePipeline.ExecuteAsync(
                async token =>
                {
                    var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync(token);

                    await using var transaction = await connection.BeginTransactionAsync(token);

                    var addSubscriptionCommand = new CommandDefinition(
                        SubscriptionSqlConstants.AddSubscription,
                        subscription,
                        transaction,
                        cancellationToken: token
                    );

                    var subscriptionSchema = await connection.QuerySingleAsync<SubscriptionSchema>(addSubscriptionCommand);

                    var addSubscriptionTopicCommand = new CommandDefinition(
                        SubscriptionSqlConstants.AddSubscriptionTopic,
                        subscription.Topics.Select(t => new { SubscriptionId = subscriptionSchema.Id, t.Name }),
                        transaction,
                        cancellationToken: token
                    );

                    var topicsInserted = await connection.ExecuteAsync(addSubscriptionTopicCommand);

                    await transaction.CommitAsync(token);

                    return new Subscription
                    {
                        Url = subscriptionSchema.Url,
                        Topics = subscription.Topics,
                    };
                },
                cancellationToken);
            }
            catch (SqlException)
            {
                throw;
            }
            catch (TimeoutException)
            {
                throw; //to do: think how best to handle this;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Result<List<string>>> GetSubscriptionUrlsByTopicName(string topicName, CancellationToken cancellationToken)
        {
            try
            {
                return await _resiliencePipeline.ExecuteAsync(
                async token =>
                {
                    var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync(token);

                    var command = new CommandDefinition(
                        SubscriptionSqlConstants.GetSubscriptionUrlsByTopicName,
                        new { TopicName = topicName },
                        cancellationToken: token
                    );

                    var urls = await connection.QueryAsync<string>(command);
                    return urls.ToList();
                },
                cancellationToken);
            }
            catch (SqlException)
            {
                throw;
            }
            catch (TimeoutException)
            {
                throw; //to do: think how best to handle this;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
