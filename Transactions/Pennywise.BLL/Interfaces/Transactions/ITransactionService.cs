using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pennywise.BLL.DTOs.Transactions;
using Pennywise.Common.Enums;

namespace Pennywise.BLL.Interfaces.Transactions
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionResponseDto>> GetAllAsync(
            string userId,
            TransactionType? type = null,
            int? accountId = null,
            int? categoryId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 50);

        Task<TransactionResponseDto?> GetByIdAsync(int id, string userId);

        Task<TransactionResponseDto> CreateAsync(CreateTransactionDto dto, string userId);

        Task UpdateAsync(int id, UpdateTransactionDto dto, string userId);

        Task DeleteAsync(int id, string userId);
    }
}
