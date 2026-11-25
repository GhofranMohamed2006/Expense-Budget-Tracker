using System;
using System.Collections.Generic;
using System.Text;

namespace Pennywise.BLL.DTOs.Budgets
{
    internal class CreateBudgetDto
    {
        public int? CategoryId { get; set; }

        public decimal Amount { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }
    }
}
