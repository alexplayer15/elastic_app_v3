namespace elastic_app_v3.background_worker.Events;

public sealed record OutboxEvent(
    Guid Id,
    string Type,
    string Payload,
    DateTime OccurredOnUtc,
    DateTime? ProcessedOnUtc,
    string? Error
);
