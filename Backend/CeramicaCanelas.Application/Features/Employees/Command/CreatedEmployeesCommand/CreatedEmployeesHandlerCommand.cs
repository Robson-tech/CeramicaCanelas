using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CeramicaCanelas.Application.Features.Employees.Command.CreatedEmployeesCommand
{
    public class CreatedEmployeesHandlerCommand : IRequestHandler<CreatedEmployeesCommand, Unit>
    {
        private readonly IEmployeesRepository _employeeRepository;
        private readonly ILogged _logged;
        public CreatedEmployeesHandlerCommand(
            IEmployeesRepository employeeRepository,
            ILogged logged)
        {
            _employeeRepository = employeeRepository;
            _logged = logged;
        }
        public async Task<Unit> Handle(CreatedEmployeesCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado");
            }

            await ValidateEmployee(request, cancellationToken);

            var pasta = Path.Combine("wwwroot", "employees", "images");
            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            const string UrlBase = "https://ceramicacanelas.shop/employees/images/";
            string? url = null;

            if (request.Imagem != null)
            {
                var nomeArquivo = $"{Guid.NewGuid()}_{request.Imagem.FileName}";
                var caminho = Path.Combine(pasta, nomeArquivo);

                using (var stream = new FileStream(caminho, FileMode.Create))
                {
                    await request.Imagem.CopyToAsync(stream);
                }

                url = $"{UrlBase}{nomeArquivo}";
            }

            var employee = request.AssignToEmployee();
            employee.ImageUrl = url;

            await _employeeRepository.CreateAsync(employee, cancellationToken);

            return Unit.Value;
        }

        public async Task ValidateEmployee(CreatedEmployeesCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreatedEmployeesValidator();

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult);
            }

        }
    }
}
