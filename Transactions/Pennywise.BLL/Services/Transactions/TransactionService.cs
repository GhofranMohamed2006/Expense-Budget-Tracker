using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pennywise.BLL.DTOs.Transactions;
using Pennywise.BLL.Interfaces.Transactions;
using Pennywise.BLL.Validators.Transactions;
using Pennywise.Common.Enums;
using Pennywise.DAL;
using Pennywise.DAL.Entities;

namespace Pennywise.BLL.Services.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _db;

        public TransactionService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<TransactionResponseDto>> GetAllAsync(
            string userId,
            TransactionType? type = null,
            int? accountId = null,
            int? categoryId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 50)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required.", nameof(userId));

            // Pagination bounds
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 100) pageSize = 100;

            var query = _db.Transactions
                .AsNoTracking()
                .Include(t => t.Account)
                .Include(t => t.TransferToAccount)
                .Include(t => t.Category)
                .Where(t => t.UserId == userId);

            if (type.HasValue)
            {
                query = query.Where(t => t.Type == type.Value);
            }

            if (accountId.HasValue)
            {
                // Include transactions where account is source or destination
                query = query.Where(t => t.AccountId == accountId.Value || t.TransferToAccountId == accountId.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate <= toDate.Value);
            }

            var transactions = await query
                .OrderByDescending(t => t.TransactionDate)
                .ThenByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionResponseDto
                {
                    Id = t.Id,
                    AccountId = t.AccountId,
                    AccountName = t.Account != null ? t.Account.Name : string.Empty,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category != null ? t.Category.Name : null,
                    TransferToAccountId = t.TransferToAccountId,
                    TransferToAccountName = t.TransferToAccount != null ? t.TransferToAccount.Name : null,
                    Type = t.Type,
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();

            return transactions;
        }

        public async Task<TransactionResponseDto?> GetByIdAsync(int id, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required.", nameof(userId));

            var t = await _db.Transactions
                .AsNoTracking()
                .Include(t => t.Account)
                .Include(t => t.TransferToAccount)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (t == null) return null;

            return new TransactionResponseDto
            {
                Id = t.Id,
                AccountId = t.AccountId,
                AccountName = t.Account != null ? t.Account.Name : string.Empty,
                CategoryId = t.CategoryId,
                CategoryName = t.Category != null ? t.Category.Name : null,
                TransferToAccountId = t.TransferToAccountId,
                TransferToAccountName = t.TransferToAccount != null ? t.TransferToAccount.Name : null,
                Type = t.Type,
                Amount = t.Amount,
                TransactionDate = t.TransactionDate,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            };
        }

        public async Task<TransactionResponseDto> CreateAsync(CreateTransactionDto dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required.", nameof(userId));

            // Basic validation
            var validation = CreateTransactionValidator.Validate(dto);
            if (!validation.IsValid)
            {
                var firstError = validation.Errors.First().Value.First();
                throw new ArgumentException(firstError);
            }

            // 1. Source Account Ownership check
            var sourceAccount = await _db.Accounts
                .FirstOrDefaultAsync(a => a.Id == dto.AccountId && a.UserId == userId);

            if (sourceAccount == null)
            {
                throw new KeyNotFoundException($"Source account with ID {dto.AccountId} was not found or does not belong to you.");
            }

            int? finalCategoryId = null;
            int? finalTransferToAccountId = null;

            if (dto.Type == TransactionType.Income)
            {
                // Category is required for Income
                if (!dto.CategoryId.HasValue)
                    throw new ArgumentException("Category is required for Income transactions.");

                var category = await ValidateAndGetCategoryAsync(dto.CategoryId.Value, userId);
                if (category.Type != CategoryType.Income)
                {
                    throw new InvalidOperationException("Selected category type must be Income for an Income transaction.");
                }

                finalCategoryId = category.Id;
                finalTransferToAccountId = null;
            }
            else if (dto.Type == TransactionType.Expense)
            {
                // Category is required for Expense
                if (!dto.CategoryId.HasValue)
                    throw new ArgumentException("Category is required for Expense transactions.");

                var category = await ValidateAndGetCategoryAsync(dto.CategoryId.Value, userId);
                if (category.Type != CategoryType.Expense)
                {
                    throw new InvalidOperationException("Selected category type must be Expense for an Expense transaction.");
                }

                finalCategoryId = category.Id;
                finalTransferToAccountId = null;
            }
            else if (dto.Type == TransactionType.Transfer)
            {
                // Transfer requires destination account
                if (!dto.TransferToAccountId.HasValue)
                    throw new ArgumentException("Transfer destination account is required.");

                if (dto.TransferToAccountId.Value == dto.AccountId)
                    throw new InvalidOperationException("Source and destination accounts must be different.");

                // Destination Account Ownership check
                var destAccount = await _db.Accounts
                    .FirstOrDefaultAsync(a => a.Id == dto.TransferToAccountId.Value && a.UserId == userId);

                if (destAccount == null)
                {
                    throw new KeyNotFoundException($"Destination account with ID {dto.TransferToAccountId.Value} was not found or does not belong to you.");
                }

                finalTransferToAccountId = destAccount.Id;

                // Category is optional for Transfer
                if (dto.CategoryId.HasValue)
                {
                    var category = await ValidateAndGetCategoryAsync(dto.CategoryId.Value, userId);
                    finalCategoryId = category.Id;
                }
            }

            var transaction = new Transaction
            {
                UserId = userId,
                AccountId = dto.AccountId,
                CategoryId = finalCategoryId,
                TransferToAccountId = finalTransferToAccountId,
                Type = dto.Type,
                Amount = dto.Amount,
                TransactionDate = dto.TransactionDate,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync();

            // Load navigation properties for response DTO
            await _db.Entry(transaction).Reference(t => t.Account).LoadAsync();
            if (transaction.TransferToAccountId.HasValue)
            {
                await _db.Entry(transaction).Reference(t => t.TransferToAccount).LoadAsync();
            }
            if (transaction.CategoryId.HasValue)
            {
                await _db.Entry(transaction).Reference(t => t.Category).LoadAsync();
            }

            return new TransactionResponseDto
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                AccountName = transaction.Account?.Name ?? string.Empty,
                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category?.Name,
                TransferToAccountId = transaction.TransferToAccountId,
                TransferToAccountName = transaction.TransferToAccount?.Name,
                Type = transaction.Type,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate,
                Description = transaction.Description,
                CreatedAt = transaction.CreatedAt,
                UpdatedAt = transaction.UpdatedAt
            };
        }

        public async Task UpdateAsync(int id, UpdateTransactionDto dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required.", nameof(userId));

            var validation = UpdateTransactionValidator.Validate(dto);
            if (!validation.IsValid)
            {
                var firstError = validation.Errors.First().Value.First();
                throw new ArgumentException(firstError);
            }

            // Find transaction owned by current user
            var transaction = await _db.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction == null)
            {
                throw new KeyNotFoundException($"Transaction with ID {id} was not found or does not belong to you.");
            }

            // Validate Category based on preserved Type
            if (dto.CategoryId.HasValue)
            {
                var category = await ValidateAndGetCategoryAsync(dto.CategoryId.Value, userId);

                if (transaction.Type == TransactionType.Income && category.Type != CategoryType.Income)
                {
                    throw new InvalidOperationException("Selected category type must be Income for this Income transaction.");
                }
                if (transaction.Type == TransactionType.Expense && category.Type != CategoryType.Expense)
                {
                    throw new InvalidOperationException("Selected category type must be Expense for this Expense transaction.");
                }

                transaction.CategoryId = category.Id;
            }
            else
            {
                if (transaction.Type == TransactionType.Income || transaction.Type == TransactionType.Expense)
                {
                    throw new ArgumentException($"A category is required for {transaction.Type} transactions.");
                }
                transaction.CategoryId = null;
            }

            // Update allowed fields
            transaction.Amount = dto.Amount;
            transaction.TransactionDate = dto.TransactionDate;
            transaction.Description = dto.Description;
            transaction.UpdatedAt = DateTime.UtcNow;

            // Preserve: Type, AccountId, TransferToAccountId, UserId
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required.", nameof(userId));

            var transaction = await _db.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction == null)
            {
                throw new KeyNotFoundException($"Transaction with ID {id} was not found or does not belong to you.");
            }

            _db.Transactions.Remove(transaction);
            await _db.SaveChangesAsync();
        }

        private async Task<Category> ValidateAndGetCategoryAsync(int categoryId, string userId)
        {
            // Category is valid when UserId == current user OR (UserId is null and IsDefault is true)
            var category = await _db.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId && (c.UserId == userId || (c.UserId == null && c.IsDefault)));

            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {categoryId} was not found or is not accessible by you.");
            }

            return category;
        }
    }
}
