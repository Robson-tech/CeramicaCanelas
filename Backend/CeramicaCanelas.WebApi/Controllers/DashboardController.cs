using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.Dashboard;
using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetFinancialQueries;
using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetInventoryStatusQueries;
using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetEmployeeMovementsQuery;
using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetOutOfStockProducts;
using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetProductsMost;
using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetProductsPeriods;
using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetUnreturnedProducts;
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

        [Authorize(Roles = "Almoxarifado,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("stock")]
        public async Task<IActionResult> GetStockPanel([FromQuery]GetInventoryStatusQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
        [Authorize(Roles = "Almoxarifado,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("reports/employees")]
        public async Task<IActionResult> GetReportsEmployees([FromQuery] GetEmployeeMovementsQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpGet("reports/products/periods")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetReportsProducts([FromQuery] GetProductReportQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpGet("reports/products/most-used")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMostUsed([FromQuery] GetMostUsedProductsQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpGet("reports/products/stock-outof")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProductsOutOf([FromQuery] GetOutOfStockProductsQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpGet("reports/products/unreturned-products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProductsUnreturned([FromQuery] GetUnreturnedProductsQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpGet("financial/monthly-cost-category")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMonthlyCostByCategory([FromQuery] GetMonthlyCostByCategoryQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpGet("primary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDashboardIndicators([FromQuery] GetDashboardIndicatorsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }











    }
}
