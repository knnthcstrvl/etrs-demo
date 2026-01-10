using Financial.DTO;

namespace Financial.Services.Interfaces
{
    public interface IProcurementService
    {
        Task<ProcurementReadDto> UpsertAsync(ProcurementCreateUpdateDto dto);
        Task<ProcurementReadDto> GetByTransactionIdAsync(int transactionId);
    }
}
