using CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.BalanceByAccountsPayReport;
using CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.BalanceByCategoryRequest;
using CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PagedRequestCashFlowReport;
using CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PagedRequestLaunchByClient;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CeramicaCanelas.WebApi.Controllers
{
    [Route("api/financial/dashboard-financial")]
    [OpenApiTags("Dashboard-financial")]

    [ApiController]
    public class DashboardFinancialController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Authorize(Roles = "Financial,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("flow-report")]
        public async Task<IActionResult> GetFlowReport([FromQuery] PagedRequestCashFlowReport query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Roles = "Financial,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("balance-expense")]
        public async Task<IActionResult> GetBalanceExpense([FromQuery] PagedRequestBalanceExpense query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Roles = "Financial,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("balance-income")]
        public async Task<IActionResult> GetBalanceIncome([FromQuery] PagedRequestBalanceIncome query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Roles = "Financial,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("launches-by-client")]
        public async Task<IActionResult> GetLaunchesByClient([FromQuery] PagedRequestLaunchByClient query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }


    }
}
