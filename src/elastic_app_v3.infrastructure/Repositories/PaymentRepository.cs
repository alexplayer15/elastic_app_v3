using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.infrastructure.Config;
using FluentResults;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Registry;
using Dapper;
using Microsoft.Data.SqlClient;
using elastic_app_v3.infrastructure.Constants;

namespace elastic_app_v3.infrastructure.Repositories
{
    public class PaymentRepository(
        IOptions<ElasticDatabaseSettings> elasticDatabaseSettings,
        ResiliencePipelineProvider<string> resiliencePipelineProvider) : IPaymentRepository
    {
        private readonly string _connectionString = elasticDatabaseSettings.Value.GetConnectionString();
        private readonly ResiliencePipeline _resiliencePipeline
            = resiliencePipelineProvider.GetPipeline(ResiliencePolicy.UserResiliencePolicyKey);
        public async Task<Result<Guid>> AddPaymentAsync(Payment payment, string idempotencyKey, CancellationToken cancellationToken)
        {
            Guid paymentId = Guid.Empty;
            try
            {
                paymentId = await _resiliencePipeline.ExecuteAsync(
                async token =>
                {
                    await using var connection = new SqlConnection(_connectionString);

                    await connection.OpenAsync(token);

                    await using var transaction = await connection.BeginTransactionAsync(token);

                    var checkIdempotencyKeyExistsCommand = new CommandDefinition(
                        IdempotencySqlConstants.CheckIfIdempotencyKeyExists,
                        new { IdempotencyKey = idempotencyKey },
                        transaction: transaction,
                        cancellationToken: token
                    );
                    var paymentExists = await connection.ExecuteScalarAsync<bool>(checkIdempotencyKeyExistsCommand);

                    if (paymentExists)
                    {
                        throw new InvalidOperationException("A payment with the same idempotency key already exists."); //temporary solution
                    }

                    var insertPaymentCommand = new CommandDefinition(
                        PaymentSqlConstants.AddPayment,
                        payment,
                        transaction: transaction,
                        cancellationToken: token);

                    paymentId = await connection.ExecuteScalarAsync<Guid>(insertPaymentCommand);

                    var insertIdempotencyKeyCommand = new CommandDefinition(
                        IdempotencySqlConstants.InsertIdempotencyKey,
                        new { IdempotencyKey = idempotencyKey },
                        transaction: transaction,
                        cancellationToken: token);

                    await connection.ExecuteAsync(insertIdempotencyKeyCommand);

                    await transaction.CommitAsync(token);

                    return paymentId;
                },
                cancellationToken);

                return paymentId;
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
