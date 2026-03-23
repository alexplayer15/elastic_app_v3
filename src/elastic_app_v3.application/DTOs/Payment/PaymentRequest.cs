namespace elastic_app_v3.application.DTOs.Payment
{
    public sealed record PaymentRequest(
        double Amount,
        string Currency,
        string Status,
        string Description
    );
}
