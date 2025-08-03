using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Customers.Queries.GetPagedCustomerQueries
{
    public class GetPagedCustomersHandler : IRequestHandler<PagedRequestCustomer, PagedResultCustomer>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetPagedCustomersHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<PagedResultCustomer> Handle(PagedRequestCustomer request, CancellationToken cancellationToken)
        {
            var all = _customerRepository.QueryAllWithIncludes();
            var filtered = all.Where(c => !c.IsDeleted);

            // Filtro de busca
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                filtered = filtered.Where(c =>
                    c.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrWhiteSpace(c.Email) && c.Email.Contains(request.Search, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(c.Document) && c.Document.Contains(request.Search, StringComparison.OrdinalIgnoreCase))
                );
            }

            // Ordenação
            filtered = request.OrderBy?.ToLower() switch
            {
                "email" => request.Ascending ? filtered.OrderBy(c => c.Email) : filtered.OrderByDescending(c => c.Email),
                "document" => request.Ascending ? filtered.OrderBy(c => c.Document) : filtered.OrderByDescending(c => c.Document),
                _ => request.Ascending ? filtered.OrderBy(c => c.Name) : filtered.OrderByDescending(c => c.Name)
            };

            var totalItems = filtered.Count();

            var pagedItems = filtered
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CustomerResult(c))
                .ToList();

            return new PagedResultCustomer
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                Items = pagedItems
            };
        }
    }
}
