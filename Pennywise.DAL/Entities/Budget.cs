using System;
using System.Collections.Generic;
using System.Text;

namespace Pennywise.DAL.Entities
{
    internal class Budget
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int? CategoryId { get; set; }

        public decimal Amount { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public DateTime CreatedAt { get; set; }
        // Navigation Properties
        //public ApplicationUser User { get; set; }

        //public Category? Category { get; set; }
    }
}
