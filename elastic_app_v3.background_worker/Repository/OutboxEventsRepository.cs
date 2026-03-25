using elastic_app_v3.background_worker.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using Dapper;
using elastic_app_v3.background_worker.Events;

namespace elastic_app_v3.background_worker.Repository;
public class OutboxEventsRepository(IOptions<ElasticDatabaseSettings> elasticDatabaseSettings) : IOutboxEventsRepository
{
    private readonly string _connectionString = elasticDatabaseSettings.Value.GetConnectionString();
    public async Task<List<OutboxEvent>> GetUnprocessedEvents(int batchSize)
    {
        using var connection = new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            @"SELECT TOP (@BatchSize) Id, Type, Payload, OccurredOnUtc, ProcessedOnUtc, Error
              FROM OutboxEvents
              WHERE ProcessedOnUtc IS NULL
              ORDER BY OccurredOnUtc",
            new { BatchSize = batchSize });

        var events = await connection.QueryAsync<OutboxEvent>(command); //this is schema not events, decouple later

        return [.. events];
    }
    public async Task MarkEventAsProcessed(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            @"UPDATE OutboxEvents
            SET ProcessedOnUtc = GETUTCDATE(),
            Error = NULL
            WHERE Id = @Id",
            new { Id = id });

        await connection.ExecuteAsync(command);
    }
    public async Task MarkEventAsFailed(Guid id, string error)
    {
        using var connection = new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            @"UPDATE OutboxEvents
            SET Error = @Error
            WHERE Id = @Id",
            new { Id = id, Error = error });

        await connection.ExecuteAsync(command);
    }
}
