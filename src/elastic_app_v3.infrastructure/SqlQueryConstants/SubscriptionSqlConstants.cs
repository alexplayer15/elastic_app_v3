namespace elastic_app_v3.infrastructure.SqlQueryConstants
{
    public class SubscriptionSqlConstants
    {
        public const string AddSubscription = @"
        INSERT INTO Subscriptions (Url)
        OUTPUT INSERTED.Id, INSERTED.Url
        VALUES (@Url);";

        public const string AddSubscriptionTopic = @"
        INSERT INTO SubscriptionTopics (SubscriptionId, Name) 
        VALUES (@SubscriptionId, @Name);";

        public const string GetSubscriptionUrlsByTopicName = @"
        SELECT s.Url
        FROM Subscriptions s
        INNER JOIN SubscriptionTopics st ON s.Id = st.SubscriptionId
        WHERE st.Name = @TopicName;";
    }
}
