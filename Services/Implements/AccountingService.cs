using Financial.Data;
using Financial.DTO;
using Financial.Model.Modules;
using Financial.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Financial.Services.Implements
{
    public class AccountingService : IAccountingService
    {
        private readonly ProcurementDbContext _context;

        public AccountingService(ProcurementDbContext context)
        {
            _context = context;
        }

        public async Task<AccountingDto> UpsertAsync(AccountingDto dto)
        {
            var entity = await _context.Accountings
                .FirstOrDefaultAsync(a => a.TransactionId == dto.TransactionId);

            if (entity == null)
            {
                entity = dto.Adapt<Accounting>();
                _context.Accountings.Add(entity);
            }
            else
            {
                dto.Adapt(entity);
            }

            await _context.SaveChangesAsync();
            return entity.Adapt<AccountingDto>();
        }

        public async Task<AccountingDto> GetByTransactionIdAsync(int transactionId)
        {
            var entity = await _context.Accountings
                .FirstOrDefaultAsync(a => a.TransactionId == transactionId);

            return entity == null ? null : entity.Adapt<AccountingDto>();
        }
    }
}
