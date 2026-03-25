namespace elastic_app_v3.domain.Events;
public sealed record UserSignedUpEvent(Guid UserId) : Event;
