namespace elastic_app_v3.domain.Events
{
    public abstract record Event
    {
        public Guid Id { get; init; } = Guid.NewGuid();
    }
}
