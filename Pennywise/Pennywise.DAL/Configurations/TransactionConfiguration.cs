using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pennywise.DAL.Entities;

namespace Pennywise.DAL.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> b)
    {
        b.ToTable("Transactions", t =>
        {
            t.HasCheckConstraint("CK_Transactions_Amount", "[Amount] > 0");
            t.HasCheckConstraint("CK_Transactions_Transfer",
                "([Type] = 'Transfer' AND [TransferToAccountId] IS NOT NULL AND [TransferToAccountId] <> [AccountId]) " +
                "OR ([Type] <> 'Transfer' AND [TransferToAccountId] IS NULL)");
        });

        b.HasKey(t => t.Id);
        b.Property(t => t.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
        b.Property(t => t.Amount).HasPrecision(18, 2);
        b.Property(t => t.Description).HasMaxLength(500);

        b.HasOne(t => t.User).WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(t => t.Account).WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(t => t.TransferToAccount).WithMany(a => a.IncomingTransfers)
            .HasForeignKey(t => t.TransferToAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(t => t.Category).WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(t => new { t.UserId, t.TransactionDate });
        b.HasIndex(t => t.AccountId);
        b.HasIndex(t => t.TransferToAccountId);
        b.HasIndex(t => t.CategoryId);
    }
}
