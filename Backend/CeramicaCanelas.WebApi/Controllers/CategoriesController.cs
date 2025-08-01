using CeramicaCanelas.Application.Features.Almoxarifado.Categories.Commands.CreatedCategoriesCommand;
using CeramicaCanelas.Application.Features.Almoxarifado.Categories.Commands.DeleteCategoriesCommand;
using CeramicaCanelas.Application.Features.Almoxarifado.Categories.Commands.UpdateCategoriesCommand;
using CeramicaCanelas.Application.Features.Almoxarifado.Categories.Queries.GetAllCategoriesQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CeramicaCanelas.WebApi.Controllers
{
    [Route("api/categories")]
    [OpenApiTags("Categories")]

    [ApiController]
    public class CategoriesController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatedCategories([FromForm] CreatedCategoriesCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCategories([FromForm] UpdateCategoriesCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCategories([FromRoute] Guid id)
        {
            var request = new DeleteCategoriesCommand { Id = id };
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCategories()
        {
            var request = new GetAllCategoriesQueries();
            var response = await _mediator.Send(request);

            return Ok(response);
        }



    }
}
