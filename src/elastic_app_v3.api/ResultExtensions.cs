using elastic_app_v3.api.Mappings;
using FluentResults;

namespace elastic_app_v3.api;
public static class ResultExtension
{
    public static IResult ToApiResponse<T>(this Result<T> result, string endpoint)
    {
        return result switch
        {
            { IsSuccess: true } => SuccessResponseMapper.GetSuccessResponseByEndpoint(result.Value, endpoint),
            { IsFailed: true, Errors: [Error error, ..] } => ErrorResponseMapper.GetErrorResponseByEndpoint(error, endpoint),
            _ => throw new InvalidOperationException("Unexpected error type")
        };
    }
    public static IResult ToApiResponse(this Result result, string endpoint)
    {
        return result switch
        {
            { IsSuccess: true } => SuccessResponseMapper.GetSuccessResponseByEndpoint(endpoint),
            { IsFailed: true, Errors: [Error error, ..] } => ErrorResponseMapper.GetErrorResponseByEndpoint(error, endpoint),
            _ => throw new InvalidOperationException("Unexpected error type")
        };
    }
    
    public static async Task<Result<T>> Tap<T>(this Task<Result<T>> resultTask, Action<T> action)
    {
        var result = await resultTask;
        
        if (result.IsSuccess)
            action(result.Value);
        
        return result;
    }
}
