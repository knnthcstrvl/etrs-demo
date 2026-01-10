using Financial.Data;
using Financial.DTO;
using Financial.Model.Trace;
using Financial.Services.Interfaces;
using Mapster;

namespace Financial.Services.Implements
{
    public class StatusHistoryService : IStatusHistoryService
    {
        private readonly ProcurementDbContext _context;

        public StatusHistoryService(ProcurementDbContext context)
        {
            _context = context;
        }

        public async Task AddStatusAsync(StatusHistoryDto dto)
        {
            var entity = dto.Adapt<StatusHistory>();
            _context.StatusHistories.Add(entity);
            await _context.SaveChangesAsync();
        }
    }
}
