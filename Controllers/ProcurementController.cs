using Financial.DTO;
using Financial.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Financial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcurementController : ControllerBase
    {
        private readonly IProcurementService _service;

        public ProcurementController(IProcurementService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create or update procurement details
        /// </summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Upsert procurement details")]
        [ProducesResponseType(typeof(ProcurementReadDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Upsert([FromBody] ProcurementCreateUpdateDto dto)
        {
            return Ok(await _service.UpsertAsync(dto));
        }

        /// <summary>
        /// Get procurement details by transaction
        /// </summary>
        [HttpGet("{transactionId}")]
        [SwaggerOperation(Summary = "Get procurement by transaction ID")]
        public async Task<IActionResult> Get(int transactionId)
        {
            var result = await _service.GetByTransactionIdAsync(transactionId);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
