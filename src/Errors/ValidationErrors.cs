using elastic_app_v3.Enums;

namespace elastic_app_v3.Errors
{
    public static class ValidationErrors
    {
        public static Error ValidationError(string errorDescription)
        {
            return new Error("Validation.ValidationError", ErrorCategory.ValidationError, errorDescription);
        }
    }
}
