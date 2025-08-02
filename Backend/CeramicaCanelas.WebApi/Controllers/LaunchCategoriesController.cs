using CeramicaCanelas.Application.Features.Almoxarifado.Product.Queries.Pages;
using CeramicaCanelas.Application.Features.Financial.FinancialBox.LaunchCategories.Commands.CreateLaunchCategoryCommand;
using CeramicaCanelas.Application.Features.Financial.FinancialBox.LaunchCategories.Commands.DeleteLaunchCategoryCommand;
using CeramicaCanelas.Application.Features.Financial.FinancialBox.LaunchCategories.Commands.UpdateLaunchCategoryCommand;
using CeramicaCanelas.Application.Features.Financial.FinancialBox.LaunchCategories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CeramicaCanelas.WebApi.Controllers
{
    [Route("api/financial/launch-categories")]
    [OpenApiTags("Launch-categories")]

    [ApiController]
    public class LaunchCategoriesController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Authorize(Roles = "Financial,Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateLaunchCategory([FromForm] CreateLaunchCategoryCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize(Roles = "Financial,Admin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateLaunchCategory([FromForm] UpdateLaunchCategoryCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize(Roles = "Financial,Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Adicionado para indicar que o item pode não ser encontrado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteLaunchCategory(Guid id)
        {
            var command = new DeleteLaunchCategoryCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();

        }

        [Authorize(Roles = "Financial,Admin")]
        [HttpGet("paged")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPagesLaunchCategories([FromQuery] PagedRequestLaunchCategory query )
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
