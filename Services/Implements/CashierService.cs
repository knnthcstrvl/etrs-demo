using Financial.Data;
using Financial.DTO;
using Financial.Model.Modules;
using Financial.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Financial.Services.Implements
{
    public class CashierService : ICashierService
    {
        private readonly ProcurementDbContext _context;

        public CashierService(ProcurementDbContext context)
        {
            _context = context;
        }

        public async Task<CashierDto> UpsertAsync(CashierDto dto)
        {
            var entity = await _context.Cashiers
                .Include(c => c.FundCluster)
                .Include(c => c.PaymentMode)
                .FirstOrDefaultAsync(c => c.TransactionId == dto.TransactionId);

            if (entity == null)
            {
                entity = dto.Adapt<Cashier>();
                _context.Cashiers.Add(entity);
            }
            else
            {
                dto.Adapt(entity);
            }

            await _context.SaveChangesAsync();
            return entity.Adapt<CashierDto>();
        }

        public async Task<CashierDto> GetByTransactionIdAsync(int transactionId)
        {
            var entity = await _context.Cashiers
                .Include(c => c.FundCluster)
                .Include(c => c.PaymentMode)
                .FirstOrDefaultAsync(c => c.TransactionId == transactionId);

            return entity == null ? null : entity.Adapt<CashierDto>();
        }
    }
}
