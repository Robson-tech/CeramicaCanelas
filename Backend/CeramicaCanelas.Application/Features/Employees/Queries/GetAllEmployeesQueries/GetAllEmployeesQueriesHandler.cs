using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;

namespace CeramicaCanelas.Application.Features.Employees.Queries.GetAllEmployeesQueries
{
    public class GetAllEmployeesQueries : IRequest<List<GetAllEmployeesQueriesResult>>
    {
        // Add any parameters needed for the query here
    }
    public class GetAllEmployeesQueriesHandler : IRequestHandler<GetAllEmployeesQueries, List<GetAllEmployeesQueriesResult>>
    {
        public readonly IEmployeesRepository _employeeRepository;
        public readonly ILogged _logged;

        public GetAllEmployeesQueriesHandler(IEmployeesRepository employeeRepository, ILogged logged)
        {
            _employeeRepository = employeeRepository;
            _logged = logged;
        }

        public async Task<List<GetAllEmployeesQueriesResult>> Handle(GetAllEmployeesQueries request, CancellationToken cancellationToken)
        {
            if (await _logged.UserLogged() == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            var employees = await _employeeRepository.GetAllAsync();
            if (!employees.Any())
            {
                throw new BadRequestException("Não há funcionários cadastrados.");
            }
            return employees.Select(employee => new GetAllEmployeesQueriesResult(employee)).ToList();
        }
    }
}
