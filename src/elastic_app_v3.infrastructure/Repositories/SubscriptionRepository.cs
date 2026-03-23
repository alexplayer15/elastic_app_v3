using Dapper;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.infrastructure.Config;
using elastic_app_v3.infrastructure.Constants;
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
        public async Task<Result<string>> SubscribeAsync(string url, CancellationToken cancellationToken)
        {
            try
            {
                return await _resiliencePipeline.ExecuteAsync(
                async token =>
                {
                    var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync(token);

                    var command = new CommandDefinition(
                        SubscriptionSqlConstants.AddSubscription,
                        new { Url = url },
                        cancellationToken: token
                    );

                    return await connection.QuerySingleAsync<string>(command);
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
