namespace elastic_app_v3.Errors
{
    public static class ResultExtensions
    {
        public static Result<U> Map<T, U>(this Result<T> result, Func<T, U> mapper)
        {
            return result.IsSuccess
                ? Result<U>.Success(mapper(result.Value))
                : Result<U>.Failure(result.Error);
        }
    }
}
