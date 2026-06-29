namespace elastic_app_v3.domain.DTOs;
public sealed record IdempotentPaymentData(
    string IdempotencyKey,
    Guid PaymentId
); 
