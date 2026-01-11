using Financial.Data;
using Financial.DTO;
using Financial.DTO.Library;
using Financial.Model.Library;
using Financial.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Financial.Services.Implements
{
    public class SupplierServices : ISupplierServices
    {
        private readonly ProcurementDbContext _context;

        public SupplierServices(ProcurementDbContext context)
        {
            _context = context;
        }
        public async Task<SupplierDto> GetBySupplierIdAsync(int supplierId)
        {

            var entity = await _context.Suppliers
                .FirstOrDefaultAsync(b => b.SupplierId == supplierId);

            return entity == null ? null : entity.Adapt<SupplierDto>();
        }

        public async Task<SupplierDto> UpsertAsync(SupplierDto dto)
        {
            var entity = dto.Adapt<Supplier>();

            _context.Suppliers.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Adapt<SupplierDto>();
        }
    }
}
