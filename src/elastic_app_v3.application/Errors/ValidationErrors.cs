using FluentResults;

namespace elastic_app_v3.application.Errors
{
    public sealed class ValidationError(string ErrorDescription) : Error()
    {
        public readonly string ErrorDescription = ErrorDescription;
    }
}
