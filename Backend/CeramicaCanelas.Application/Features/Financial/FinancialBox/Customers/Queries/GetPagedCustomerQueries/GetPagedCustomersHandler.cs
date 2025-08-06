using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

            // Filtro de busca (CORRIGIDO)
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchTerm = request.Search.ToLower();
                filtered = filtered.Where(c =>
                    c.Name.ToLower().Contains(searchTerm) ||
                    (c.Email != null && c.Email.ToLower().Contains(searchTerm)) ||
                    (c.Document != null && c.Document.ToLower().Contains(searchTerm))
                );
            }


            // Ordenação (mantida como estava)
            filtered = request.OrderBy?.ToLower() switch
            {
                "email" => request.Ascending ? filtered.OrderBy(c => c.Email) : filtered.OrderByDescending(c => c.Email),
                "document" => request.Ascending ? filtered.OrderBy(c => c.Document) : filtered.OrderByDescending(c => c.Document),
                _ => request.Ascending ? filtered.OrderBy(c => c.Name) : filtered.OrderByDescending(c => c.Name)
            };

            // A contagem e paginação devem ocorrer após todos os filtros
            var totalItems = await filtered.CountAsync(cancellationToken); // Use CountAsync para operações assíncronas

            var pagedItems = await filtered
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CustomerResult(c))
                .ToListAsync(cancellationToken); // Use ToListAsync para operações assíncronas

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
