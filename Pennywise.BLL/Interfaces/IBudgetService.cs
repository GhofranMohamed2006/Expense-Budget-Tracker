using Pennywise.BLL.DTOs.Budgets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pennywise.BLL.Interface
{
    internal interface IBudgetService
    {
        Task<IEnumerable<BudgetResponseDto>> GetAllAsync(int month, int year);

        Task<BudgetResponseDto?> GetByIdAsync(int id);

        Task<BudgetResponseDto> CreateAsync(CreateBudgetDto dto);

        Task<bool> UpdateAsync(int id, UpdateBudgetDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
