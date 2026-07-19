using elastic_app_v3.api.Errors;
using elastic_app_v3.api.Routing.Constants;
using elastic_app_v3.domain.Errors;
using elastic_app_v3.domain.Errors.Identity;
using elastic_app_v3.domain.Errors.Profile;
using Microsoft.AspNetCore.Mvc;

namespace elastic_app_v3.api.Mappings;
public static class ErrorResponseMapper
{
    private static readonly Dictionary<string, Func<BaseError, IResult>> _errorResponseMap = new()
    {
        { EndpointConstants.UserSignUpEndpoint, GetSignUpErrorResponse },
        { EndpointConstants.UserLoginEndpoint, GetLoginErrorResponse  },
        { EndpointConstants.GetUserByIdEndpoint, GetUserByIdErrorResponse },
        { EndpointConstants.PaymentEndpoint, GetPaymentErrorResponse },
        { EndpointConstants.UpdateProfileEndpoint, GetUpdateProfileErrorResponse },
        { EndpointConstants.GetProfilePictureUrls, GetProfilePictureUrlErrorResponse },
        { EndpointConstants.SaveProfilePicture, GetSaveProfilePictureErrorResponse }
    };
    public static IResult GetErrorResponseByEndpoint(
        BaseError internalError,
        string endpoint)
    {
        return _errorResponseMap.TryGetValue(endpoint, out var mapper)
            ? mapper(internalError)
            : throw new InvalidOperationException(
                $"Error response mapping has not been configured for endpoint '{endpoint}'");
    }
    
    //LOTS of duplicate code - clean up here
    private static IResult GetSignUpErrorResponse(BaseError error)
    {
        (int statusCode, string errorCode) = error switch
        {
            UserAlreadyExistsError => (StatusCodes.Status409Conflict, ErrorCodes.UserAlreadyExistsError),
            _ => (StatusCodes.Status500InternalServerError, ErrorCodes.UnknownError)
        };

        var problemDetails = new ProblemDetails
        {
            Type = errorCode,
            Title = "An error occurred during user sign-up.",
            Detail = error.Message,
            Status = statusCode
        };

        return Results.Json(problemDetails, statusCode: statusCode);
    }
    private static IResult GetLoginErrorResponse(BaseError error)
    {
        (int statusCode, string errorCode) = error switch
        {
            UserDoesNotExistError => (StatusCodes.Status404NotFound, ErrorCodes.UserDoesNotExistError),
            IncorrectPasswordError => (StatusCodes.Status401Unauthorized, ErrorCodes.IncorrectPasswordError),
            _ => (StatusCodes.Status500InternalServerError, ErrorCodes.UnknownError)
        };

        var problemDetails = new ProblemDetails
        {
            Type = errorCode,
            Title = "An error occurred during user login.",
            Detail = error.Message,
            Status = statusCode
        };

        return Results.Json(problemDetails, statusCode: statusCode);
    }
    private static IResult GetUserByIdErrorResponse(BaseError error)
    {
        (int statusCode, string errorCode) = error switch
        {
            UserDoesNotExistError => (StatusCodes.Status404NotFound, ErrorCodes.UserDoesNotExistError),
            _ => (StatusCodes.Status500InternalServerError, ErrorCodes.UnknownError)
        };

        var problemDetails = new ProblemDetails
        {
            Type = errorCode,
            Title = "An error occurred retrieving user.",
            Detail = error.Message,
            Status = statusCode
        };

        return Results.Json(problemDetails, statusCode: statusCode);
    }
    private static IResult GetPaymentErrorResponse(BaseError error)
    {
        (int statusCode, string errorCode) = error switch
        {
            _ => (StatusCodes.Status500InternalServerError, ErrorCodes.UnknownError)
        };

        var problemDetails = new ProblemDetails
        {
            Type = errorCode,
            Title = "An error occurred during payment.",
            Detail = error.Message,
            Status = statusCode
        };

        return Results.Json(problemDetails, statusCode: statusCode);
    }
    private static IResult GetUpdateProfileErrorResponse(BaseError error)
    {
        (int statusCode, string errorCode) = error switch
        {
            NoProfileFoundError => (StatusCodes.Status404NotFound, ErrorCodes.NoProfileFoundError),
            _ => (StatusCodes.Status500InternalServerError, ErrorCodes.UnknownError)
        };

        var problemDetails = new ProblemDetails
        {
            Type = errorCode,
            Title = "An error occurred during update profile",
            Detail = error.Message,
            Status = statusCode
        };

        return Results.Json(problemDetails, statusCode: statusCode);
    }
    private static IResult GetProfilePictureUrlErrorResponse(BaseError error)
    {
        (int statusCode, string errorCode) = error switch
        {
            _ => (StatusCodes.Status500InternalServerError, ErrorCodes.UnknownError)
        };

        var problemDetails = new ProblemDetails
        {
            Type = errorCode,
            Title = "An error occurred retrieving profile picture url.",
            Detail = error.Message,
            Status = statusCode
        };

        return Results.Json(problemDetails, statusCode: statusCode);
    }
    private static IResult GetSaveProfilePictureErrorResponse(BaseError error)
    {
        (int statusCode, string errorCode) = error switch
        {
            _ => (StatusCodes.Status500InternalServerError, ErrorCodes.UnknownError)
        };

        var problemDetails = new ProblemDetails
        {
            Type = errorCode,
            Title = "An error occurred saving profile picture.",
            Detail = error.Message,
            Status = statusCode
        };

        return Results.Json(problemDetails, statusCode: statusCode);
    }
}
