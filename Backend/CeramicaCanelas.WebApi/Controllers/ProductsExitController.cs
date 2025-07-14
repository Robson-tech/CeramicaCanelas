using CeramicaCanelas.Application.Features.Movimentacoes_ES.Entradas.Command.CreateMovEntradasProductsCommand;
using CeramicaCanelas.Application.Features.Movimentacoes_ES.Entradas.Command.DeleteMovEntradasProductsCommand;
using CeramicaCanelas.Application.Features.Movimentacoes_ES.Entradas.Queries.GetAllProductsEntryQueries;
using CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Command.CreateMovExitProductsCommand;
using CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Command.UpdateMovExitProdructsCommand;
using CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CeramicaCanelas.WebApi.Controllers
{
    [Route("api/products-exit")]
    [ApiController]
    [OpenApiTags("products-exit")]
    public class ProductsExitController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Authorize(Roles = "Custoumer,Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatedProductsExit([FromForm] CreateMovExitProductsCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Custoumer,Admin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProductsExit([FromForm] UpdateMovExitProdructsCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Custoumer,Admin")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteProductsExit([FromQuery] DeleteMovEntradasProductsCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Custoumer,Admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProductsExit()
        {
            var request = new GetAllProductsExitQueries();
            var response = await _mediator.Send(request);
            return Ok(response);

        }
    }
}
