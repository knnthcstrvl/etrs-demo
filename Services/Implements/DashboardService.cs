using Financial.Data;
using Financial.DTO.Report;
using Financial.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Financial.Services.Implements
{
    public class DashboardService : IDashboardService
    {
        private readonly ProcurementDbContext _context;

        public DashboardService(ProcurementDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardKpiDto> GetSummaryKpisAsync()
        {
            var today = DateTime.Today;

            // PROCUREMENT KPIS
            var totalPrs = await _context.Procurements.CountAsync();
            var prsCompleted = await _context.Procurements
                .CountAsync(p => p.Status == "Completed");
            var prsPending = await _context.Procurements
                .CountAsync(p => p.Status != "Completed");
            var prsDelayed = await _context.Procurements
                .CountAsync(p => p.LastDayDeliveryOrEventDate.HasValue &&
                                 p.LastDayDeliveryOrEventDate < today &&
                                 p.Status != "Completed");

            // DELIVERY KPIS
            var totalDeliveries = await _context.Properties.CountAsync();
            var deliveriesDelayed = await _context.Properties
                .CountAsync(p => p.LastDayDeliveryTerm.HasValue &&
                                 p.DeliveryDate.HasValue &&
                                 p.DeliveryDate > p.LastDayDeliveryTerm);
            var deliveriesOnTime = totalDeliveries - deliveriesDelayed;

            // ACCOUNTING / PAYMENT KPIS
            var totalDv = await _context.Accountings.CountAsync();
            var dvPaid = await _context.Accountings
                .CountAsync(a => a.Status == "Paid");
            var dvPending = totalDv - dvPaid;
            var totalNetAmount = await _context.Accountings.SumAsync(a => a.NetAmount ?? 0);
            var totalOverdueAmount = await _context.Accountings
                .Where(a => a.Status != "Paid" && a.DvDateReceived.HasValue)
                .SumAsync(a => a.NetAmount ?? 0);

            return new DashboardKpiDto
            {
                TotalPrs = totalPrs,
                PrsCompleted = prsCompleted,
                PrsPending = prsPending,
                PrsDelayed = prsDelayed,
                TotalDeliveries = totalDeliveries,
                DeliveriesOnTime = deliveriesOnTime,
                DeliveriesDelayed = deliveriesDelayed,
                TotalDv = totalDv,
                DvPaid = dvPaid,
                DvPending = dvPending,
                TotalNetAmount = totalNetAmount,
                TotalOverdueAmount = totalOverdueAmount
            };
        }
    }
}
