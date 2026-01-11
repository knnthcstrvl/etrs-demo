using Financial.DTO;
using Financial.DTO.Library;
using Financial.Model.Library;

namespace Financial.Services.Interfaces
{
    public interface ISupplierServices
    {
        Task<SupplierDto> UpsertAsync(SupplierDto dto);
        Task<SupplierDto> GetBySupplierIdAsync(int supplierId);
    }
}
