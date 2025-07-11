using CeramicaCanelas.Application.Features.Movimentacoes_ES.Entradas.Command.CreateMovEntradasProductsCommand;
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
    public class ProductsEntry(IMediator mediator) : ControllerBase
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



    }
}


