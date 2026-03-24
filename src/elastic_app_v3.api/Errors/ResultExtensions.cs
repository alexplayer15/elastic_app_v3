using FluentResults;

namespace elastic_app_v3.api.Errors
{
    public static class ResultExtension
    {
        public static IResult ToApiResponse<T>(this Result<T> result, string endpoint)
        {
            return result switch
            {
                { IsSuccess: true } => TypedResults.Ok(result.Value),
                { IsFailed: true, Errors: [Error error, ..] } => ErrorResponseMapper.GetErrorResponseByEndpoint(error, endpoint),
                _ => throw new InvalidOperationException("Unexpected error type")
            };
        }
        public static IResult ToApiResponse(this Result result, string endpoint)
        {
            return result switch
            {
                { IsSuccess: true } => TypedResults.Ok(result),
                { IsFailed: true, Errors: [Error error, ..] } => ErrorResponseMapper.GetErrorResponseByEndpoint(error, endpoint),
                _ => throw new InvalidOperationException("Unexpected error type")
            };
        }
    }
}
