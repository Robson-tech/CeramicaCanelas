using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetInventoryStatusQueries;
using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetEmployeeMovementsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CeramicaCanelas.WebApi.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Authorize(Roles = "Custoumer,Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("stock")]
        public async Task<IActionResult> GetStockPanel([FromQuery]GetInventoryStatusQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
        [Authorize(Roles = "Custoumer,Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("reports/employees")]
        public async Task<IActionResult> GetReportsEmployees([FromQuery] GetEmployeeMovementsQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }





    }
}
