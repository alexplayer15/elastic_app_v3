using elastic_app_v3.api.Routing.Constants;
using Microsoft.AspNetCore.Http.HttpResults;

namespace elastic_app_v3.api.Mappings;
public class SuccessResponseMapper
{
    private static readonly Dictionary<string, Func<IResult>> _successResponseMapNoValue = new()
    {
        { EndpointConstants.UserSignUpEndpoint, GetSignUpSuccessResponse }
    };

    private static readonly Dictionary<string, Func<object, IResult>> _successResponseMapWithValue = new()
    {
        { EndpointConstants.UserLoginEndpoint, value => GetLoginSuccessResponse(value) },
        { EndpointConstants.GetUserByIdEndpoint, value => GetUserByIdSuccessResponse(value) },
        { EndpointConstants.PaymentEndpoint, value => GetPaymentSuccessResponse(value) }
    };
    public static IResult GetSuccessResponseByEndpoint(string endpoint)
    {
        return _successResponseMapNoValue.TryGetValue(endpoint, out var mapper)
            ? mapper()
            : throw new InvalidOperationException($"No success response configured for endpoint '{endpoint}'");
    }
    public static IResult GetSuccessResponseByEndpoint<T>(T value, string endpoint)
    {
        return _successResponseMapWithValue.TryGetValue(endpoint, out var mapper)
            ? mapper(value!)
            : throw new InvalidOperationException($"No success response configured for endpoint '{endpoint}'");
    }
    private static Created GetSignUpSuccessResponse() => TypedResults.Created();
    private static Ok<T> GetLoginSuccessResponse<T>(T value) => TypedResults.Ok(value);
    private static Ok<T> GetUserByIdSuccessResponse<T>(T value) => TypedResults.Ok(value);
    private static Ok<T> GetPaymentSuccessResponse<T>(T value) => TypedResults.Ok(value);
}
