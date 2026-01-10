using Financial.DTO;

namespace Financial.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionReadDto> CreateAsync(TransactionCreateDto dto);
        Task<TransactionReadDto> GetByIdAsync(int transactionId);
        Task<IEnumerable<TransactionReadDto>> GetAllAsync();
    }
}
