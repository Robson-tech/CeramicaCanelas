using CeramicaCanelas.Application.Features.Financial.FinancialBox.Customers.Commands.CreateCustomerCommand;
using CeramicaCanelas.Application.Features.Financial.FinancialBox.Customers.Commands.DeleteCustomerCommand;
using CeramicaCanelas.Application.Features.Financial.FinancialBox.Customers.Commands.UpdateCustomerCommand;
using CeramicaCanelas.Application.Features.Financial.FinancialBox.Customers.Queries.GetPagedCustomerQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CeramicaCanelas.WebApi.Controllers
{
    [Route("api/financial/customer")]
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

        [Authorize(Roles = "Financial,Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Adicionado para indicar que o item pode não ser encontrado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var command = new DeleteCustomerCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize(Roles = "Financial,Admin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCustomer([FromForm] UpdateCustomerCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize(Roles = "Financial,Admin")]
        [HttpGet("paged")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPagedCustomers([FromQuery] PagedRequestCustomer query ,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

    }
}
