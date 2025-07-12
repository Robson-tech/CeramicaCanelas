using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;

namespace CeramicaCanelas.Application.Features.Employees.Command.UpdateEmployeesCommand
{
    public class UpdateEmployeesCommandHandle : IRequestHandler<UpdateEmployeesCommand, Unit>
    {
        private readonly IEmployeesRepository _employeeRepository;
        private readonly ILogged _logged;

        public UpdateEmployeesCommandHandle(
            IEmployeesRepository employeeRepository,
            ILogged logged)
        {
            _employeeRepository = employeeRepository;
            _logged = logged;
        }

        public async Task<Unit> Handle(UpdateEmployeesCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            var employee = await ValidateUpdateEmployee(request, cancellationToken);

            var pasta = Path.Combine("wwwroot", "employees", "images");
            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            const string UrlBase = "https://localhost:7014/employees/images/";
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

            employee.Name = request.Name;
            employee.CPF = request.CPF;
            employee.Positiions = request.Positiions;
            employee.ImageUrl = url;
            employee.ModifiedOn = DateTime.UtcNow;

            await _employeeRepository.Update(employee);

            return Unit.Value;

        }

        public async Task<Domain.Entities.Employee> ValidateUpdateEmployee(UpdateEmployeesCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.Id);

            if (employee == null)
            {
                throw new BadRequestException($"Funcionário não encontrado.");
            }      

            var validator = new UpdateEmployeesCommandValidator();

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult);
            }


            return employee;

        }
    }
}
