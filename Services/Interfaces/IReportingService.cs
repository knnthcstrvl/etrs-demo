using Financial.DTO.Report;

namespace Financial.Services.Interfaces
{
    public interface IReportingService
    {
        Task<IEnumerable<ProcurementAgingReportDto>> GetProcurementAgingAsync(int thresholdDays);
        Task<IEnumerable<DeliveryDelayReportDto>> GetDeliveryDelaysAsync();
        Task<IEnumerable<AccountingAgingReportDto>> GetAccountingAgingAsync(int thresholdDays);
    }
}
