using elastic_app_v3.background_worker.Events;

namespace elastic_app_v3.background_worker.Repository;
public interface IOutboxEventsRepository
{
    Task<List<OutboxEvent>> GetUnprocessedEvents(int batchSize);
    Task MarkEventAsProcessed(Guid id);
    Task MarkEventAsFailed(Guid id, string error);
}
