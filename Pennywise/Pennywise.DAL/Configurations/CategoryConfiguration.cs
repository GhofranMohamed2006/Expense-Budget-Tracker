using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pennywise.Common.Enums;
using Pennywise.DAL.Entities;

namespace Pennywise.DAL.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    private static readonly DateTime SeedDate = new(2026, 11, 25, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<Category> b)
    {
        b.ToTable("Categories");
        b.HasKey(c => c.Id);
        b.Property(c => c.Name).IsRequired().HasMaxLength(100);
        b.Property(c => c.Type).HasConversion<string>().HasMaxLength(20).IsRequired();

        // UserId nullable => shared default categories
        b.HasOne(c => c.User).WithMany(u => u.Categories)
            .HasForeignKey(c => c.UserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(c => c.UserId);

        b.HasData(
            Seed(1, "Food", CategoryType.Expense), Seed(2, "Transport", CategoryType.Expense),
            Seed(3, "Shopping", CategoryType.Expense), Seed(4, "Entertainment", CategoryType.Expense),
            Seed(5, "Bills", CategoryType.Expense), Seed(6, "Health", CategoryType.Expense),
            Seed(7, "Education", CategoryType.Expense), Seed(8, "Other Expense", CategoryType.Expense),
            Seed(9, "Salary", CategoryType.Income), Seed(10, "Freelance", CategoryType.Income),
            Seed(11, "Gift", CategoryType.Income), Seed(12, "Other Income", CategoryType.Income));
    }

    private static Category Seed(int id, string name, CategoryType type) => new()
    {
        Id = id, UserId = null, Name = name, Type = type, IsDefault = true, CreatedAt = SeedDate
    };
}
