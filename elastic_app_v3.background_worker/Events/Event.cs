namespace elastic_app_v3.background_worker.Events;
public abstract record Event
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
