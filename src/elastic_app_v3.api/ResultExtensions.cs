using elastic_app_v3.api.Mappings;
using CSharpFunctionalExtensions;
using elastic_app_v3.domain.Errors;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace elastic_app_v3.api;
public static class ResultExtension
{
    public static IResult ToApiResponse<T, TE>(this Result<T, TE> result, string endpoint) where TE : BaseError
    {
        return result switch
        {
            { IsSuccess: true } => SuccessResponseMapper.GetSuccessResponseByEndpoint(result.Value, endpoint),
            { IsFailure: true, Error: { } error } => ErrorResponseMapper.GetErrorResponseByEndpoint(error, endpoint),
            _ => throw new InvalidOperationException("Unexpected error type")
        };
    }
    public static IResult ToApiResponse<TE>(this UnitResult<TE> result, string endpoint) where TE : BaseError
    {
        return result switch
        {
            { IsSuccess: true } => SuccessResponseMapper.GetSuccessResponseByEndpoint(endpoint),
            { IsFailure: true, Error: { } error } => ErrorResponseMapper.GetErrorResponseByEndpoint(error, endpoint),
            _ => throw new InvalidOperationException("Unexpected error type")
        };
    }
}
