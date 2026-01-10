using Financial.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Financial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportingService _service;

        public ReportsController(IReportingService service)
        {
            _service = service;
        }

        [HttpGet("procurement-aging")]
        public async Task<IActionResult> ProcurementAging([FromQuery] int days = 7)
            => Ok(await _service.GetProcurementAgingAsync(days));

        [HttpGet("delivery-delays")]
        public async Task<IActionResult> DeliveryDelays()
            => Ok(await _service.GetDeliveryDelaysAsync());

        [HttpGet("accounting-aging")]
        public async Task<IActionResult> AccountingAging([FromQuery] int days = 15)
            => Ok(await _service.GetAccountingAgingAsync(days));
    }
}
