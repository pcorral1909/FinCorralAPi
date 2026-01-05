using FinCorralApi.Application.DTOs;
using FinCorralApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinCorralApi.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("resumen")]
        public async Task<ActionResult<DashboardResumenDto>> GetResumen()
        {
            var resumen = await _dashboardService.ObtenerResumenAsync();
            return Ok(resumen);
        }
    }

}
