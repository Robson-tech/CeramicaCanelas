using CeramicaCanelas.Application.Features.Almoxarifado.Product.Commands.CreatedProductCommand;
using CeramicaCanelas.Application.Features.Almoxarifado.Product.Commands.DeleteProductCommand;
using CeramicaCanelas.Application.Features.Almoxarifado.Product.Commands.UpdateProductCommand;
using CeramicaCanelas.Application.Features.Almoxarifado.Product.Queries.GetAllProductsQueries;
using CeramicaCanelas.Application.Features.Almoxarifado.Product.Queries.Pages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CeramicaCanelas.WebApi.Controllers
{
    [Route("api/products")]
    [OpenApiTags("Products")]
    [ApiController]
    public class ProductsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpPost]
        [RequestSizeLimit(20_000_000)] // Limita para 10MB por request (ajustável)
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatedProduct([FromForm] CreatedProductCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpPut]
        [RequestSizeLimit(20_000_000)] // Limita para 10MB por request (ajustável)
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
        {
            var request = new DeleteProductCommand { Id = id };
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProducts()
        {
            var request = new GetAllProductsQueries();
            var response = await _mediator.Send(request);
            return Ok(response);

        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<GetAllProductsQueriesResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PagedResult<GetAllProductsQueriesResult>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPagedProducts([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}
