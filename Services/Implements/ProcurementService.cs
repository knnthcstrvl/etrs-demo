using Financial.Data;
using Financial.DTO;
using Financial.Model.Modules;
using Financial.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Financial.Services.Implements
{
    public class ProcurementService : IProcurementService
    {
        private readonly ProcurementDbContext _context;

        public ProcurementService(ProcurementDbContext context)
        {
            _context = context;
        }

        public async Task<ProcurementReadDto> UpsertAsync(ProcurementCreateUpdateDto dto)
        {
            var entity = await _context.Procurements
                .Include(p => p.Transaction)
                .Include(p => p.Mode)
                .Include(p => p.Type)
                .FirstOrDefaultAsync(p => p.TransactionId == dto.TransactionId);

            if (entity == null)
            {
                // DTO -> Entity
                entity = dto.Adapt<Procurement>();
                _context.Procurements.Add(entity);
            }
            else
            {
                // Update existing entity
                dto.Adapt(entity);
            }

            await _context.SaveChangesAsync();

            // Entity -> Read DTO
            var result = entity.Adapt<ProcurementReadDto>();
            return result;
        }

        public async Task<ProcurementReadDto> GetByTransactionIdAsync(int transactionId)
        {
            var entity = await _context.Procurements
                .Include(p => p.Transaction)
                .Include(p => p.Mode)
                .Include(p => p.Type)
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);

            if (entity == null)
                return null;

            var result = entity.Adapt<ProcurementReadDto>();
            return result;
        }
    }
}
