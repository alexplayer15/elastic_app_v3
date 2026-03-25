namespace elastic_app_v3.infrastructure.SqlQueryConstants
{
    public static class OutboxEventSqlConstants
    {
        public const string AddUserSignedUpEvent = @"
        INSERT INTO OutboxEvents (Type, Payload) 
        VALUES (@Type, @Payload);";
    }
}
