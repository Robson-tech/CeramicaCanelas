using CeramicaCanelas.Application.Features.Financial.FinancialBox.Customers.CreateCustomerCommand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CeramicaCanelas.WebApi.Controllers
{
    [Route("api/customer")]
    [OpenApiTags("Customer")]

    [ApiController]
    public class CustomerController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Authorize(Roles = "Financial,Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateCustomer([FromForm] CreateCustomerCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command);
            return NoContent();
        }

    }
}
