using CeramicaCanelas.Application.Features.Movimentacoes_ES.Entradas.Command.CreateMovEntradasProductsCommand;
using CeramicaCanelas.Application.Features.Movimentacoes_ES.Entradas.Command.DeleteMovEntradasProductsCommand;
using CeramicaCanelas.Application.Features.Movimentacoes_ES.Entradas.Command.UpdateMovEntradasProductsCommand;
using CeramicaCanelas.Application.Features.Movimentacoes_ES.Entradas.Queries.GetAllProductsEntryQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CeramicaCanelas.WebApi.Controllers
{
    [Route("api/products-entry")]
    [ApiController]
    [OpenApiTags("products-entry")]
    public class ProductsEntryController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Authorize(Roles = "Custoumer,Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatedProductsEntry([FromForm] CreateMovEntradasProductCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }


        [Authorize(Roles = "Custoumer,Admin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProductsEntry([FromForm] UpdateMovEntradasProductsCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Custoumer,Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteProductsEntry([FromRoute] Guid id)
        {
            var request = new DeleteMovEntradasProductsCommand { Id = id };
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Custoumer,Admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProductsEntry()
        {
            var request = new GetAllProductsEntryQueries();
            var response = await _mediator.Send(request);
            return Ok(response);

        }



    }
}


