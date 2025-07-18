using CeramicaCanelas.Application.Features.Suppliers.Commands.CreatedSuppliersCommand;
using CeramicaCanelas.Application.Features.Suppliers.Queries.Pages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CeramicaCanelas.WebApi.Controllers
{
    [Route("api/supplier")]
    [OpenApiTags("Supplier")]
    [ApiController]
    public class SupplierRepository(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;


        [Authorize(Roles = "Custoumer,Admin")]
        [HttpGet("paged")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPagedSuppliers([FromQuery] PagedRequestSupplier request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [Authorize(Roles = "Custoumer,Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSupplier([FromForm] CreatedSuppliersCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }
    }
}
