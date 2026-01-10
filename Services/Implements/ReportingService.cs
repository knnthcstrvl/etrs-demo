using Financial.Data;
using Financial.DTO.Report;
using Financial.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Financial.Services.Implements
{
    public class ReportingService : IReportingService
    {
        private readonly ProcurementDbContext _context;

        public ReportingService(ProcurementDbContext context)
        {
            _context = context;
        }

        // ================================
        // PROCUREMENT AGING
        // ================================
        public async Task<IEnumerable<ProcurementAgingReportDto>> GetProcurementAgingAsync(int thresholdDays)
        {
            var today = DateTime.Today;

            return await _context.Procurements
                .Include(p => p.Transaction)
                .Select(p => new ProcurementAgingReportDto
                {
                    TransactionId = p.TransactionId,
                    PrNumber = p.Transaction.PrNumber,
                    EndUser = p.EndUser,
                    Status = p.Status,
                    PrDateReceived = p.PrDateReceived,
                    LastDayDelivery = p.LastDayDeliveryOrEventDate,
                    AgingDays = p.PrDateReceived.HasValue
                        ? EF.Functions.DateDiffDay(p.PrDateReceived.Value, today)
                        : 0,
                    IsDelayed = p.LastDayDeliveryOrEventDate.HasValue
                        && p.LastDayDeliveryOrEventDate.Value < today
                        && p.Status != "Completed"
                })
                .Where(r => r.AgingDays >= thresholdDays)
                .OrderByDescending(r => r.AgingDays)
                .ToListAsync();
        }

        // ================================
        // DELIVERY DELAYS (PROPERTY)
        // ================================
        public async Task<IEnumerable<DeliveryDelayReportDto>> GetDeliveryDelaysAsync()
        {
            var today = DateTime.Today;

            return await _context.Properties
                .Include(p => p.Transaction)
                    .ThenInclude(t => t.Supplier)
                .Select(p => new DeliveryDelayReportDto
                {
                    TransactionId = p.TransactionId,
                    ContractNumber = p.Transaction.ContractNumber,
                    SupplierName = p.Transaction.Supplier.SupplierName,
                    DeliveryDueDate = p.LastDayDeliveryTerm,
                    ActualDeliveryDate = p.DeliveryDate,
                    DelayDays = p.LastDayDeliveryTerm.HasValue
                        ? EF.Functions.DateDiffDay(
                            p.LastDayDeliveryTerm.Value,
                            p.DeliveryDate ?? today)
                        : 0,
                    Status = p.Status
                })
                .Where(r => r.DelayDays > 0 && r.Status != "Complete")
                .OrderByDescending(r => r.DelayDays)
                .ToListAsync();
        }

        // ================================
        // ACCOUNTING AGING (DV / PAYMENT)
        // ================================
        public async Task<IEnumerable<AccountingAgingReportDto>> GetAccountingAgingAsync(int thresholdDays)
        {
            var today = DateTime.Today;

            return await _context.Accountings
                .Include(a => a.Transaction)
                    .ThenInclude(t => t.Cashier)
                .Select(a => new AccountingAgingReportDto
                {
                    TransactionId = a.TransactionId,
                    DvNumber = a.Transaction.DvNumber,
                    Payee = a.Transaction.Cashier.Payee,
                    DvDateReceived = a.DvDateReceived,
                    NetAmount = a.NetAmount,
                    AgingDays = a.DvDateReceived.HasValue
                        ? EF.Functions.DateDiffDay(a.DvDateReceived.Value, today)
                        : 0,
                    Status = a.Status
                })
                .Where(r => r.AgingDays >= thresholdDays && r.Status != "Paid")
                .OrderByDescending(r => r.AgingDays)
                .ToListAsync();
        }
    }
}
