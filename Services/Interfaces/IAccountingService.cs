using Financial.DTO;

namespace Financial.Services.Interfaces
{
    public interface IAccountingService
    {
        Task<AccountingDto> UpsertAsync(AccountingDto dto);
        Task<AccountingDto> GetByTransactionIdAsync(int transactionId);
    }
}
