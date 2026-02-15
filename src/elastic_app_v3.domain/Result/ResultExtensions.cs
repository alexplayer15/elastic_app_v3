namespace elastic_app_v3.domain.Result
{
    public static class ResultExtensions
    {
        public static Result<U> Map<T, U>(this Result<T> result, Func<T, U> mapper)
        {
            return result.IsSuccess
                ? Result<U>.Success(mapper(result.Value))
                : Result<U>.Failure(result.Error);
        }
        public static Result<U> MapError<T, U>(this Result<T> result, Func<T, U> mapper)
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException("Cannot map error on a successful result");
            }
            return Result<U>.Failure(result.Error);
        }
    }
}
