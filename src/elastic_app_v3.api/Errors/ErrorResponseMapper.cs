using elastic_app_v3.api.Routing;
using elastic_app_v3.application.Errors;
using FluentResults;

namespace elastic_app_v3.api.Errors
{
    public static class ErrorResponseMapper
    {
        private static readonly Dictionary<string, Func<Error, IResult>> _errorResponseMap = new()
        {
            { RoutingConstants.UserSignUpEndpoint, GetSignUpErrorResponse },
            { RoutingConstants.UserLoginEndpoint, GetLoginErrorResponse  },
            { RoutingConstants.GetUserByIdEndpoint, GetUserByIdErrorResponse },
            { RoutingConstants.PaymentEndpoint, GetPaymentErrorResponse },
            { RoutingConstants.SubscribeEndpoint, GetSubscribeErrorResponse },
            {RoutingConstants.PublishEndpoint, GetPublishErrorResponse }
        };
        public static IResult GetErrorResponseByEndpoint(
            Error internalError,
            string endpoint)
        {
            return _errorResponseMap.TryGetValue(endpoint, out var mapper)
                ? mapper(internalError)
                : throw new InvalidOperationException(
                    $"Error response mapping has not been configured for endpoint '{endpoint}'");
        }
        private static IResult GetSignUpErrorResponse(Error internalError)
        {
            (int statusCode, string errorCode) = internalError switch
            {
                ValidationError => (StatusCodes.Status400BadRequest, ErrorCodes.ValidationError),
                UserAlreadyExistsError => (StatusCodes.Status409Conflict, ErrorCodes.UserAlreadyExistsError),
                _ => (StatusCodes.Status500InternalServerError, ErrorCodes.UnknownError)
            };

            var apiError = new ApiError(errorCode, internalError.Message);

            return Results.Json(apiError, statusCode: statusCode);
        }
        private static IResult GetLoginErrorResponse(Error internalError)
        {
            (int statusCode, string errorCode) = internalError switch
            {
                ValidationError => (StatusCodes.Status400BadRequest, ErrorCodes.ValidationError),
                UserDoesNotExistError => (StatusCodes.Status404NotFound, ErrorCodes.UserDoesNotExistError),
                IncorrectPasswordError => (StatusCodes.Status401Unauthorized, ErrorCodes.IncorrectPasswordError),
                _ => (StatusCodes.Status500InternalServerError, ErrorCodes.UnknownError)
            };

            var apiError = new ApiError(errorCode, internalError.Message);

            return Results.Json(apiError, statusCode: statusCode);
        }
        private static IResult GetUserByIdErrorResponse(Error internalError)
        {
            (int statusCode, string errorCode) = internalError switch
            {
                UserDoesNotExistError => (StatusCodes.Status404NotFound, ErrorCodes.UserDoesNotExistError),
                _ => (StatusCodes.Status500InternalServerError, ErrorCodes.UnknownError)
            };

            var apiError = new ApiError(errorCode, internalError.Message);

            return Results.Json(apiError, statusCode: statusCode);
        }
        private static IResult GetPaymentErrorResponse(Error internalError)
        {
            (int statusCode, string errorCode) = internalError switch
            {
                _ => (StatusCodes.Status500InternalServerError, ErrorCodes.UnknownError)
            };

            var apiError = new ApiError(errorCode, internalError.Message);

            return Results.Json(apiError, statusCode: statusCode);
        }
        private static IResult GetSubscribeErrorResponse(Error internalError)
        {
            (int statusCode, string errorCode) = internalError switch
            {
                _ => (StatusCodes.Status500InternalServerError, ErrorCodes.UnknownError)
            };
            var apiError = new ApiError(errorCode, internalError.Message);
            return Results.Json(apiError, statusCode: statusCode);
        }
        private static IResult GetPublishErrorResponse(Error internalError)
        {
            (int statusCode, string errorCode) = internalError switch
            {
                _ => (StatusCodes.Status500InternalServerError, ErrorCodes.UnknownError)
            };
            var apiError = new ApiError(errorCode, internalError.Message);
            return Results.Json(apiError, statusCode: statusCode);
        }
    }
}
