namespace elastic_app_v3.infrastructure.Constants
{
    public static class IdempotencySqlConstants
    {
        public const string CheckIfIdempotencyKeyExists = @"
        SELECT CAST(
            CASE WHEN EXISTS (SELECT 1 FROM IdempotencyKeys WHERE IdempotencyKey = @IdempotencyKey)
            THEN 1 ELSE 0 END AS BIT
        );";

        public const string InsertIdempotencyKey = @"
        INSERT INTO IdempotencyKeys (IdempotencyKey)
        VALUES (@IdempotencyKey);";
    }
}
