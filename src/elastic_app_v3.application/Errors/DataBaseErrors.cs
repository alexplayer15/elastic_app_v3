using elastic_app_v3.domain.Result;

namespace elastic_app_v3.application.Errors
{
    public static class DataBaseErrors
    {
        public static Error SqlDatabaseError(string message)
        {
            return new Error("Database.SqlException", ErrorCategory.SqlException, message);
        }

        public static Error SqlTimeoutError(string message)
        {
            return new Error("Database.SqlTimeoutException", ErrorCategory.SqlTimeoutException, message); //mixing up exceptions and errors here, think about what makes more sense
        }
    }
}
