using Pennywise.Common.Enums;

namespace Pennywise.DAL.Entities;

public class Transaction
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    /// <summary>Source account.</summary>
    public int AccountId { get; set; }
    public int? CategoryId { get; set; }
    /// <summary>Destination account (Transfer only).</summary>
    public int? TransferToAccountId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Account Account { get; set; } = null!;
    public Category? Category { get; set; }
    public Account? TransferToAccount { get; set; }
}
