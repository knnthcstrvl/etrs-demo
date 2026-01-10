using Financial.DTO;

namespace Financial.Services.Interfaces
{
    public interface IBudgetService
    {
        Task<BudgetDto> UpsertAsync(BudgetDto dto);
        Task<BudgetDto> GetByTransactionIdAsync(int transactionId);
    }
}
