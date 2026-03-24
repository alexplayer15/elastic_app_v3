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
using elastic_app_v3.infrastructure.Models;

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

                    var insertPaymentCommand = new CommandDefinition(
                        PaymentSqlConstants.AddPayment,
                        payment,
                        transaction,
                        cancellationToken: token);

                    paymentId = await connection.ExecuteScalarAsync<Guid>(insertPaymentCommand);

                    var idempotencyKeySchema = new IdempotentPaymentData(idempotencyKey, paymentId);

                    var insertIdempotencyKeyCommand = new CommandDefinition(
                        IdempotencySqlConstants.InsertIdempotencyKey,
                        idempotencyKeySchema,
                        transaction,
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
        public async Task<Result<Guid>> CheckIfIdempotencyKeyExists(string idempotencyKey, CancellationToken cancellation)
        {
            try
            {
                var idempotencyRecord = await _resiliencePipeline.ExecuteAsync(
                async token =>
                {
                    await using var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync(token);
                    var checkIdempotencyKeyExistsCommand = new CommandDefinition(
                        IdempotencySqlConstants.CheckIfIdempotencyKeyExists,
                        new { IdempotencyKey = idempotencyKey },
                        cancellationToken: token
                    );
                    return await connection.QueryFirstOrDefaultAsync<IdempotencyKeySchema>(checkIdempotencyKeyExistsCommand);
                },
                cancellation);

                if (idempotencyRecord is not null
                          && idempotencyRecord.IdempotencyKey is not null
                          && DateTime.UtcNow - idempotencyRecord.CreatedAt <= TimeSpan.FromMinutes(10))
                {
                    return idempotencyRecord.PaymentId;
                }

                return Result.Ok<Guid>(Guid.Empty);
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
