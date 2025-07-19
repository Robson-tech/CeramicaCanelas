using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Employees.Queries.Pages
{
    public class GetPagedEmployeesHandler : IRequestHandler<PagedRequestEmployees, PagedResultEmployees>
    {
        private readonly IEmployeesRepository _employeeRepository;

        public GetPagedEmployeesHandler(IEmployeesRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<PagedResultEmployees> Handle(PagedRequestEmployees request, CancellationToken cancellationToken)
        {
            var all = await _employeeRepository.GetAllAsync();

            // Filtros
            if (!string.IsNullOrWhiteSpace(request.Search))
                all = all.Where(e => e.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase)).ToList();

            if (request.Positions.HasValue)
                all = all.Where(e => e.Positiions == request.Positions).ToList();

            if (request.ActiveOnly.HasValue && request.ActiveOnly.Value)
                all = all.Where(e => e.IsActive).ToList();

            // Ordenação
            all = request.OrderBy?.ToLower() switch
            {
                "name" => request.Ascending ? all.OrderBy(e => e.Name).ToList() : all.OrderByDescending(e => e.Name).ToList(),
                "role" => request.Ascending ? all.OrderBy(e => e.Positiions).ToList() : all.OrderByDescending(e => e.Positiions).ToList(),
                _ => all.OrderBy(e => e.Name).ToList()
            };

            var totalItems = all.Count;

            var pagedItems = all
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new EmployeeResult(e))
                .ToList();

            return new PagedResultEmployees
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                Items = pagedItems
            };
        }
    }
}
