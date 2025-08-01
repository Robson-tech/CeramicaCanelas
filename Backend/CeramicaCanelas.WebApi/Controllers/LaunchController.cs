using CeramicaCanelas.Application.Features.Financial.FinancialBox.Launches.Commands.CreatedLaunchCommand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CeramicaCanelas.WebApi.Controllers
{
    [Route("api/launch")]
    [OpenApiTags("Launch")]

    [ApiController]
    public class LaunchController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Authorize(Roles = "Financial,Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatedLaunch([FromForm] CreatedLaunchCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }

    }
}
