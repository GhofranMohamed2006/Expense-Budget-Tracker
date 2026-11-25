using System;
using Pennywise.Common.Enums;

namespace Pennywise.BLL.DTOs.Transactions
{
    public class TransactionResponseDto
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public string AccountName { get; set; } = string.Empty;

        public int? CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public int? TransferToAccountId { get; set; }

        public string? TransferToAccountName { get; set; }

        public TransactionType Type { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
