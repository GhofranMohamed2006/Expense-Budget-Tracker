using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pennywise.DAL.Entities;

namespace Pennywise.DAL.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> b)
    {
        b.ToTable("Accounts");
        b.HasKey(a => a.Id);
        b.Property(a => a.Name).IsRequired().HasMaxLength(100);
        b.Property(a => a.AccountType).HasConversion<string>().HasMaxLength(20).IsRequired();
        b.Property(a => a.OpeningBalance).HasPrecision(18, 2);
        b.Property(a => a.IsActive).HasDefaultValue(true);

        b.HasOne(a => a.User).WithMany(u => u.Accounts)
            .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(a => a.UserId);
    }
}
