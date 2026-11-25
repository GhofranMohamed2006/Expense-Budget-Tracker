using System;
using System.Collections.Generic;
using System.Text;

namespace Pennywise.BLL.DTOs.Budgets
{
    internal class BudgetResponseDto
    {
        public int Id { get; set; }

        public int? CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public decimal Amount { get; set; }

        public decimal SpentAmount { get; set; }

        public decimal RemainingAmount { get; set; }

        public double UsagePercentage { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }
    }
}
