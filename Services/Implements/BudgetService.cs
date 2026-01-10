
using Financial.Data;
using Financial.DTO;
using Financial.Model.Modules;
using Financial.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Financial.Services.Implements
{
    public class BudgetService : IBudgetService
    {
        private readonly ProcurementDbContext _context;

        public BudgetService(ProcurementDbContext context)
        {
            _context = context;
        }

        public async Task<BudgetDto> UpsertAsync(BudgetDto dto)
        {
            var entity = await _context.Budgets
                .Include(b => b.FundCluster)
                .FirstOrDefaultAsync(b => b.TransactionId == dto.TransactionId);

            if (entity == null)
            {
                entity = dto.Adapt<Budget>();
                _context.Budgets.Add(entity);
            }
            else
            {
                dto.Adapt(entity);
            }

            await _context.SaveChangesAsync();
            var result = entity.Adapt<BudgetDto>();
            return result;
        }

        public async Task<BudgetDto> GetByTransactionIdAsync(int transactionId)
        {
            var entity = await _context.Budgets
                .Include(b => b.FundCluster)
                .FirstOrDefaultAsync(b => b.TransactionId == transactionId);

            return entity == null ? null : entity.Adapt<BudgetDto>();
        }
    }
}
