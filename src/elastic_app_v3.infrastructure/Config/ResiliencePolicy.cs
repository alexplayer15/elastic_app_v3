using System.ComponentModel.DataAnnotations;

namespace elastic_app_v3.infrastructure.Config
{
    public class ResiliencePolicy
    {
        public const string UserResiliencePolicyKey = "ResiliencePolicySettings";

        public const string UserResiliencePolicySettings = "UserResiliencePolicySettings";
        public double Delay { get; set; }

        [Range(1, 1000)]
        public int TimeoutInMilliseconds { get; set; }

        [Range(1, 500)]
        public int InnerTimeoutInMilliseconds { get; set; }
        public int MaxRetryAttempts { get; set; }
        public double FailureRatio { get; set; }
        public int SampleDuration { get; set; }
        public int MinimumThroughput { get; set; }
        public int BreakDuration { get; set; }
    }
}
