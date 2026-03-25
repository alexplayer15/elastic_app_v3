using elastic_app_v3.background_worker.Producer;
using elastic_app_v3.background_worker.Repository;

namespace elastic_app_v3.background_worker.OutboxWorker;

public class OutboxWorker(
    IOutboxEventsRepository outboxEventsRepository,
    IEventProducer eventProducer) : BackgroundService
{
    private readonly IOutboxEventsRepository _outboxEventsRepository = outboxEventsRepository;
    private readonly IEventProducer _eventProducer = eventProducer;
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var events = await _outboxEventsRepository.GetUnprocessedEvents(10);

                foreach (var @event in events)
                {
                    try
                    {
                        await _eventProducer.PublishAsync(@event.Type, @event.Payload);

                        await _outboxEventsRepository.MarkEventAsProcessed(@event.Id);
                    }
                    catch (Exception ex)
                    {
                        // log + persist failure
                        await _outboxEventsRepository.MarkEventAsFailed(@event.Id, ex.Message);
                    }
                }
            }
            catch (Exception)
            {
                throw; //to do: handle failure modes
            }

            await Task.Delay(1000, cancellationToken);
        }
    }
}
