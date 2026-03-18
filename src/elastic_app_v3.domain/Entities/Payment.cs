namespace elastic_app_v3.domain.Entities
{
    public sealed class Payment
    {
        public Guid Id {  get; set; }
        public double Amount { get; set; }
        public string Currency {  get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; //change to Enum when needed
    }
}
