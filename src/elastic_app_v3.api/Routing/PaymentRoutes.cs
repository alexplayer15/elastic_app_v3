using elastic_app_v3.api.Routing.Constants;
using elastic_app_v3.application.DTOs.Payment;
using elastic_app_v3.application.Services.Payments;
using Microsoft.AspNetCore.Mvc;

namespace elastic_app_v3.api.Routing;
public static class PaymentRoutes
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost(EndpointConstants.PaymentEndpoint, async Task<IResult> (
            [FromBody] PaymentRequest request,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
            [FromServices] IPaymentService paymentService,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(idempotencyKey))
                return Results.BadRequest("Missing Idempotency-Key header");

            var result = await paymentService.AddPayment(request, idempotencyKey, cancellationToken);
            return result.ToApiResponse(EndpointConstants.PaymentEndpoint);
        })
        .MapToApiVersion(1);
    }
}
