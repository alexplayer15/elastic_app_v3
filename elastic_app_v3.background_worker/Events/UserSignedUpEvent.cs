namespace elastic_app_v3.background_worker.Events;
public sealed record UserSignedUpEvent(Guid UserId) : Event;
