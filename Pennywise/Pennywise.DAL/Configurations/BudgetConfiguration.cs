using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pennywise.DAL.Entities;

namespace Pennywise.DAL.Configurations;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> b)
    {
        b.ToTable("Budgets", t =>
        {
            t.HasCheckConstraint("CK_Budgets_Amount", "[Amount] > 0");
            t.HasCheckConstraint("CK_Budgets_Month", "[Month] BETWEEN 1 AND 12");
        });

        b.HasKey(x => x.Id);
        b.Property(x => x.Amount).HasPrecision(18, 2);

        b.HasOne(x => x.User).WithMany(u => u.Budgets)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Category).WithMany(c => c.Budgets)
            .HasForeignKey(x => x.CategoryId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

        // One budget per user + category + month + year
        b.HasIndex(x => new { x.UserId, x.CategoryId, x.Month, x.Year })
            .IsUnique().HasFilter("[CategoryId] IS NOT NULL")
            .HasDatabaseName("UX_Budgets_User_Category_Month_Year");

        // One overall budget per user + month + year (CategoryId = NULL)
        b.HasIndex(x => new { x.UserId, x.Month, x.Year })
            .IsUnique().HasFilter("[CategoryId] IS NULL")
            .HasDatabaseName("UX_Budgets_User_Overall_Month_Year");
    }
}
