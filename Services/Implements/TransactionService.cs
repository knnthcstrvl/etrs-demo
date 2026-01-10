using Financial.Data;
using Financial.DTO;
using Financial.Model;
using Financial.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Financial.Services.Implements
{
    public class TransactionService : ITransactionService
    {
        private readonly ProcurementDbContext _context;

        public TransactionService(ProcurementDbContext context)
        {
            _context = context;
        }

        public async Task<TransactionReadDto> CreateAsync(TransactionCreateDto dto)
        {
            // Convert DTO -> Entity
            var entity = dto.Adapt<Transaction>();

            _context.Transactions.Add(entity);
            await _context.SaveChangesAsync();

            // Load Supplier for mapping
            await _context.Entry(entity)
                .Reference(t => t.Supplier)
                .LoadAsync();

            // Convert Entity -> Read DTO
            var result = entity.Adapt<TransactionReadDto>();
            return result;
        }

        public async Task<TransactionReadDto> GetByIdAsync(int transactionId)
        {
            var entity = await _context.Transactions
                .Include(t => t.Supplier)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (entity == null)
                return null;

            var result = entity.Adapt<TransactionReadDto>();
            return result;
        }

        public async Task<IEnumerable<TransactionReadDto>> GetAllAsync()
        {
            var entities = await _context.Transactions
                .Include(t => t.Supplier)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var result = entities.Adapt<IEnumerable<TransactionReadDto>>();
            return result;
        }
    }
}
