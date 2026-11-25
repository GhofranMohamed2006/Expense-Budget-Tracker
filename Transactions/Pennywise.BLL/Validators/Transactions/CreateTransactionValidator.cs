using System;
using System.Collections.Generic;
using Pennywise.BLL.DTOs.Transactions;
using Pennywise.Common.Enums;

namespace Pennywise.BLL.Validators.Transactions
{
    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();

        public void AddError(string propertyName, string errorMessage)
        {
            if (Errors.TryGetValue(propertyName, out var existing))
            {
                var list = new List<string>(existing) { errorMessage };
                Errors[propertyName] = list.ToArray();
            }
            else
            {
                Errors[propertyName] = new[] { errorMessage };
            }
        }
    }

    public static class CreateTransactionValidator
    {
        public static ValidationResult Validate(CreateTransactionDto dto)
        {
            var result = new ValidationResult();

            if (dto.AccountId <= 0)
            {
                result.AddError(nameof(dto.AccountId), "AccountId must be greater than zero.");
            }

            if (dto.Amount <= 0)
            {
                result.AddError(nameof(dto.Amount), "Amount must be greater than zero.");
            }

            if (!Enum.IsDefined(typeof(TransactionType), dto.Type))
            {
                result.AddError(nameof(dto.Type), "Invalid transaction type.");
            }

            if (dto.TransactionDate == default)
            {
                result.AddError(nameof(dto.TransactionDate), "A valid transaction date is required.");
            }

            if (dto.Description != null && dto.Description.Length > 500)
            {
                result.AddError(nameof(dto.Description), "Description cannot exceed 500 characters.");
            }

            // Rules for specific types
            if (dto.Type == TransactionType.Income || dto.Type == TransactionType.Expense)
            {
                if (!dto.CategoryId.HasValue || dto.CategoryId.Value <= 0)
                {
                    result.AddError(nameof(dto.CategoryId), $"{dto.Type} transaction requires a category.");
                }

                if (dto.TransferToAccountId.HasValue)
                {
                    result.AddError(nameof(dto.TransferToAccountId), "TransferToAccountId must be null for Income and Expense transactions.");
                }
            }
            else if (dto.Type == TransactionType.Transfer)
            {
                if (!dto.TransferToAccountId.HasValue || dto.TransferToAccountId.Value <= 0)
                {
                    result.AddError(nameof(dto.TransferToAccountId), "TransferToAccountId is required for transfers.");
                }
                else if (dto.TransferToAccountId.Value == dto.AccountId)
                {
                    result.AddError(nameof(dto.TransferToAccountId), "Source and destination accounts must be different.");
                }
            }

            return result;
        }
    }
}
