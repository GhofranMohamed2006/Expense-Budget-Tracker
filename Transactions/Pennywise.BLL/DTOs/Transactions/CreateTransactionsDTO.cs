using System;
using System.ComponentModel.DataAnnotations;
using Pennywise.Common.Enums;

namespace Pennywise.BLL.DTOs.Transactions
{
    public class CreateTransactionDto
    {
        [Required(ErrorMessage = "AccountId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "AccountId must be a valid positive integer.")]
        public int AccountId { get; set; }

        public int? CategoryId { get; set; }

        public int? TransferToAccountId { get; set; }

        [Required(ErrorMessage = "Transaction type is required.")]
        [EnumDataType(typeof(TransactionType), ErrorMessage = "Type must be a valid TransactionType (Income, Expense, Transfer).")]
        public TransactionType Type { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "TransactionDate is required.")]
        public DateTime TransactionDate { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
