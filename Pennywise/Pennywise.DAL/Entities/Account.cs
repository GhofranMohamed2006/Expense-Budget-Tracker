using Pennywise.Common.Enums;

namespace Pennywise.DAL.Entities;

public class Account
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public decimal OpeningBalance { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
    /// <summary>Transactions where this account is the source (AccountId).</summary>
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    /// <summary>Transfers where this account is the destination (TransferToAccountId).</summary>
    public ICollection<Transaction> IncomingTransfers { get; set; } = new List<Transaction>();
}
