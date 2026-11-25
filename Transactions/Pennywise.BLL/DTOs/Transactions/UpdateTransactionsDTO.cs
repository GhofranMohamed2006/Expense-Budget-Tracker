using System;
using System.ComponentModel.DataAnnotations;

namespace Pennywise.BLL.DTOs.Transactions
{
    public class UpdateTransactionDto
    {
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "TransactionDate is required.")]
        public DateTime TransactionDate { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
