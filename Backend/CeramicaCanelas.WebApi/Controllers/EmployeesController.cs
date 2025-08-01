using CeramicaCanelas.Application.Features.Employees.Command.CreatedEmployeesCommand;
using CeramicaCanelas.Application.Features.Employees.Command.DeleteEmployeesCommand;
using CeramicaCanelas.Application.Features.Employees.Command.UpdateEmployeesCommand;
using CeramicaCanelas.Application.Features.Employees.Queries.GetAllEmployeesQueries;
using CeramicaCanelas.Application.Features.Employees.Queries.Pages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CeramicaCanelas.WebApi.Controllers
{
    [Route("api/employees")]
    [OpenApiTags("Employees")]
    [ApiController]
    public class EmployeesController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatedEmployee([FromForm] CreatedEmployeesCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEmployee([FromForm] UpdateEmployeesCommand request)
        {
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteEmployee([FromRoute] Guid id)
        {
            var request = new DeleteEmployeesCommand { Id = id };
            await _mediator.Send(request);
            return NoContent();
        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEmployees()
        {
            var request = new GetAllEmployeesQueries();
            var response = await _mediator.Send(request);
            return Ok(response);

        }

        [Authorize(Roles = "Almoxarifado,Admin")]
        [HttpGet("pages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEmployeesPages([FromQuery] PagedRequestEmployees request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

    }
}
