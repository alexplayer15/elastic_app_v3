namespace elastic_app_v3.infrastructure.SqlQueryConstants
{
    public static class IdempotencySqlConstants
    {
        public const string CheckIfIdempotencyKeyExists = @"
        SELECT IdempotencyKey, PaymentId, CreatedAt
        FROM IdempotencyKeys
        WHERE IdempotencyKey = @IdempotencyKey;";

        public const string InsertIdempotencyKey = @"
        INSERT INTO IdempotencyKeys (IdempotencyKey, PaymentId)
        VALUES (@IdempotencyKey, @PaymentId);";
    }
}
