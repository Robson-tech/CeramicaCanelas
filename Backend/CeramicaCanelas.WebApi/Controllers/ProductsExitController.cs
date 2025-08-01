using CeramicaCanelas.Application.Features.Almoxarifado.Movimentacoes_ES.Saidas.Queries.Pages;
using CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Command.CreateMovExitProductsCommand;
using CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Command.DeleteMovExitProdructsCommand;
using CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Command.ReturnProductExitCommand;
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

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatedProductsExit([FromForm] CreateMovExitProductsCommand request)
        {
            await _mediator.Send(request);
            return NoContent();

        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProductsExit([FromForm] UpdateMovExitProdructsCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteProductsExit([FromRoute] Guid id)
        {
            var request = new DeleteMovExitProdructsCommand { Id = id };
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProductsExit()
        {
            var request = new GetAllProductsExitQueries();
            var response = await _mediator.Send(request);
            return Ok(response);

        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpPut("returned-products")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReturnProductsExit([FromForm] ReturnProductExitCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpGet("paged")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPagedProductsExit([FromQuery] PagedRequestProductsExit request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);

        }
    }
}
