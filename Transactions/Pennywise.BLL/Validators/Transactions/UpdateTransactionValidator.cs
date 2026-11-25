using System;
using Pennywise.BLL.DTOs.Transactions;

namespace Pennywise.BLL.Validators.Transactions
{
    public static class UpdateTransactionValidator
    {
        public static ValidationResult Validate(UpdateTransactionDto dto)
        {
            var result = new ValidationResult();

            if (dto.Amount <= 0)
            {
                result.AddError(nameof(dto.Amount), "Amount must be greater than zero.");
            }

            if (dto.TransactionDate == default)
            {
                result.AddError(nameof(dto.TransactionDate), "A valid transaction date is required.");
            }

            if (dto.Description != null && dto.Description.Length > 500)
            {
                result.AddError(nameof(dto.Description), "Description cannot exceed 500 characters.");
            }

            if (dto.CategoryId.HasValue && dto.CategoryId.Value <= 0)
            {
                result.AddError(nameof(dto.CategoryId), "CategoryId must be a positive integer.");
            }

            return result;
        }
    }
}
