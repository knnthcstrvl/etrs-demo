using Financial.DTO;
using Financial.Hubs;
using Financial.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;

namespace Financial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _service;
        private readonly IHubContext<TransactionNotificationHub> _hub;

        public TransactionsController(ITransactionService service, IHubContext<TransactionNotificationHub> hub)
        {
            _service = service;
            _hub = hub;
        }

        [HttpPost]
        [SwaggerOperation(
        Summary = "Create transaction",
        Description = "Creates a new transaction anchored by PR Number"
            )]
        [ProducesResponseType(typeof(TransactionReadDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(TransactionCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            await _hub.Clients.All.SendAsync("TransactionUpdated", result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get transaction by ID")]
        [ProducesResponseType(typeof(TransactionReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
