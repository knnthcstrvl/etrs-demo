using Financial.DTO.Report;

namespace Financial.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardKpiDto> GetSummaryKpisAsync();
    }
}
