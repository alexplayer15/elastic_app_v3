namespace elastic_app_v3.infrastructure.Constants
{
    public class SubscriptionSqlConstants
    {
        public const string AddSubscription = @"
        INSERT INTO Subscriptions (Url)
        OUTPUT INSERTED.Url
        VALUES (@Url);";
    }
}
