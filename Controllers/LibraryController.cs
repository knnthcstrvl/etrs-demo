using Financial.DTO;
using Financial.DTO.Library;
using Financial.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Financial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly ISupplierServices _supplierService;

        public LibraryController(ISupplierServices supplierServices)
        {
            _supplierService = supplierServices;
            
        }

        [HttpPost]
        public async Task<IActionResult> Create(SupplierDto dto)
        {
            var result = await _supplierService.UpsertAsync(dto);
            return Ok(result);
        }
    }
}
