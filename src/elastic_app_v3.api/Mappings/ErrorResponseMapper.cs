using elastic_app_v3.api.Errors;
using elastic_app_v3.api.Routing.Constants;
using elastic_app_v3.application.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace elastic_app_v3.api.Mappings;
public static class ErrorResponseMapper
{
    private static readonly Dictionary<string, Func<Error, IResult>> _errorResponseMap = new()
    {
        { EndpointConstants.UserSignUpEndpoint, GetSignUpErrorResponse },
        { EndpointConstants.UserLoginEndpoint, GetLoginErrorResponse  },
        { EndpointConstants.GetUserByIdEndpoint, GetUserByIdErrorResponse },
        { EndpointConstants.PaymentEndpoint, GetPaymentErrorResponse },
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

        var problemDetails = new ProblemDetails
        {
            Type = errorCode,
            Title = "An error occurred during user sign-up.",
            Detail = internalError.Message,
            Status = statusCode
        };

        return Results.Json(problemDetails, statusCode: statusCode);
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

        var problemDetails = new ProblemDetails
        {
            Type = errorCode,
            Title = "An error occurred during user login.",
            Detail = internalError.Message,
            Status = statusCode
        };

        return Results.Json(problemDetails, statusCode: statusCode);
    }
    private static IResult GetUserByIdErrorResponse(Error internalError)
    {
        (int statusCode, string errorCode) = internalError switch
        {
            UserDoesNotExistError => (StatusCodes.Status404NotFound, ErrorCodes.UserDoesNotExistError),
            _ => (StatusCodes.Status500InternalServerError, ErrorCodes.UnknownError)
        };

        var problemDetails = new ProblemDetails
        {
            Type = errorCode,
            Title = "An error occurred retrieving user.",
            Detail = internalError.Message,
            Status = statusCode
        };

        return Results.Json(problemDetails, statusCode: statusCode);
    }
    private static IResult GetPaymentErrorResponse(Error internalError)
    {
        (int statusCode, string errorCode) = internalError switch
        {
            _ => (StatusCodes.Status500InternalServerError, ErrorCodes.UnknownError)
        };

        var problemDetails = new ProblemDetails
        {
            Type = errorCode,
            Title = "An error occurred during payment.",
            Detail = internalError.Message,
            Status = statusCode
        };

        return Results.Json(problemDetails, statusCode: statusCode);
    }
}
