using CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PagedRequestCashFlowReport;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CeramicaCanelas.WebApi.Controllers
{
    [Route("api/dashboard-financial")]
    [OpenApiTags("Dashboard-financial")]

    [ApiController]
    public class DashboardFinancial(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Authorize(Roles = "Custoumer,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("flow-report")]
        public async Task<IActionResult> GetFlowReport([FromQuery] PagedRequestCashFlowReport query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

    }
}
