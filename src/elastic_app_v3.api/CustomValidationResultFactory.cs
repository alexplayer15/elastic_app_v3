using elastic_app_v3.api.Errors;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Results;

namespace elastic_app_v3.api;
public class CustomValidationResultFactory : IFluentValidationAutoValidationResultFactory
{
    public IResult CreateResult(EndpointFilterInvocationContext context, ValidationResult validationResult)
    {
        var errorDescription = string.Join("; ",
            validationResult.Errors.Select(e => e.ErrorMessage));

        var problemDetails = new ProblemDetails
        {
            Type = ErrorCodes.ValidationError,
            Title = "An error occurred during request validation.",
            Detail = errorDescription,
            Status = StatusCodes.Status400BadRequest
        };

        return Results.Json(problemDetails, statusCode: StatusCodes.Status400BadRequest);
    }
}
