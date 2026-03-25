namespace elastic_app_v3.infrastructure.SqlQueryConstants
{
    public static class PaymentSqlConstants
    {
        public const string AddPayment = @"
        INSERT INTO Payments (Amount, Currency, Status)
        OUTPUT INSERTED.Id
        VALUES (@Amount, @Currency, @Status);";
    }
}
