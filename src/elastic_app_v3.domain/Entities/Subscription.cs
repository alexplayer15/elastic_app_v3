namespace elastic_app_v3.domain.Entities
{
    public class Subscription
    {
        public string Url { get; set; } = string.Empty;
        public List<Topic> Topics { get; set; } = [];
    }
}
