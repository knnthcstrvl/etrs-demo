using Financial.DTO;

namespace Financial.Services.Interfaces
{
    public interface IStatusHistoryService
    {
        Task AddStatusAsync(StatusHistoryDto dto);
    }
}
