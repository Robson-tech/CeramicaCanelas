using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;

namespace CeramicaCanelas.Application.Features.Employees.Command.DeleteEmployeesCommand
{
    public class DeleteEmployeesCommandHandler : IRequestHandler<DeleteEmployeesCommand, Unit>
    {
        private readonly IEmployeesRepository _employeeRepository;
        private readonly ILogged _logged;

        public DeleteEmployeesCommandHandler(
            IEmployeesRepository employeeRepository,
            ILogged logged)
        {
            _employeeRepository = employeeRepository;
            _logged = logged;
        }

        public async Task<Unit> Handle(DeleteEmployeesCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }
            var employee = await _employeeRepository.GetByIdAsync(request.Id);
            if (employee == null)
            {
                throw new BadRequestException("Funcionário não encontrado.");
            }
            await _employeeRepository.Delete(employee);

            return Unit.Value;
        }

    }
}
