using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pennywise.DAL.Entities;

namespace Pennywise.DAL.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            // Primary key
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedOnAdd();

            // Required Properties
            builder.Property(t => t.UserId)
                .IsRequired();

            builder.Property(t => t.AccountId)
                .IsRequired();

            // Decimal Precision
            builder.Property(t => t.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // Enum as string or int - conversion to string for clear DB readability or default int
            builder.Property(t => t.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(t => t.TransactionDate)
                .IsRequired();

            // Description maximum length 500
            builder.Property(t => t.Description)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.UpdatedAt)
                .IsRequired(false);

            // Relationships with DeleteBehavior.Restrict
            // 1. User relationship
            builder.HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. Source Account relationship
            builder.HasOne(t => t.Account)
                .WithMany(a => a.SourceTransactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Destination Account relationship (TransferToAccount)
            builder.HasOne(t => t.TransferToAccount)
                .WithMany(a => a.DestinationTransactions)
                .HasForeignKey(t => t.TransferToAccountId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Optional Category relationship
            builder.HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for optimal querying
            builder.HasIndex(t => t.UserId);
            builder.HasIndex(t => t.AccountId);
            builder.HasIndex(t => t.TransferToAccountId);
            builder.HasIndex(t => t.CategoryId);
            builder.HasIndex(t => t.TransactionDate);
            builder.HasIndex(t => new { t.UserId, t.TransactionDate });
        }
    }
}
