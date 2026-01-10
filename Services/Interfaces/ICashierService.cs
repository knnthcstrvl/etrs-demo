using Financial.DTO;

namespace Financial.Services.Interfaces
{
    public interface ICashierService
    {
        Task<CashierDto> UpsertAsync(CashierDto dto);
        Task<CashierDto> GetByTransactionIdAsync(int transactionId);
    }
}
