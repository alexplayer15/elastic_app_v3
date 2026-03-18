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
        IOptions<PaymentSettings> paymentSettings,
        ResiliencePipelineProvider<string> resiliencePipelineProvider) : IPaymentRepository
    {
        private readonly string _connectionString = paymentSettings.Value.GetConnectionString();
        private readonly ResiliencePipeline _resiliencePipeline
            = resiliencePipelineProvider.GetPipeline(ResiliencePolicy.UserResiliencePolicyKey);
        public async Task<Result<Guid>> AddPayment(Payment payment, CancellationToken cancellationToken)
        {
            Guid paymentId = Guid.Empty;
            try
            {
                paymentId = await _resiliencePipeline.ExecuteAsync(
                async token =>
                {
                    await using var connection = new SqlConnection(_connectionString);

                    await connection.OpenAsync(token);

                    var command = new CommandDefinition(
                        PaymentSqlConstants.AddPayment,
                        new { Payment = payment },
                        cancellationToken: token);

                    return await connection.ExecuteScalarAsync<Guid>(command);
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
