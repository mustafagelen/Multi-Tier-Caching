namespace Domain.Transactions;

public class Transaction
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Description { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string Status { get; set; } = "Completed";
    public string MerchantName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
