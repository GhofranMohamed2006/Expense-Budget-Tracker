namespace Pennywise.DAL.Entities;

public class Budget
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    /// <summary>null = overall monthly budget.</summary>
    public int? CategoryId { get; set; }
    public decimal Amount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
    public Category? Category { get; set; }
}
