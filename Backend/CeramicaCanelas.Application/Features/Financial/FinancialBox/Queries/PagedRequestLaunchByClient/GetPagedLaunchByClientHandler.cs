using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PagedRequestLaunchByClient
{
    public class GetPagedLaunchByClientHandler : IRequestHandler<PagedRequestLaunchByClient, PagedResultLaunchByClient>
    {
        private readonly ILaunchRepository _launchRepository;

        public GetPagedLaunchByClientHandler(ILaunchRepository launchRepository)
        {
            _launchRepository = launchRepository;
        }

        public async Task<PagedResultLaunchByClient> Handle(PagedRequestLaunchByClient request, CancellationToken cancellationToken)
        {
            var launches = _launchRepository.QueryAllWithIncludes();

            // Apenas lançamentos do tipo Entrada e com cliente associado
            var filtered = launches
                .Where(l => l.Type == LaunchType.Income && l.CustomerId != null)
                .AsQueryable();

            if (request.CustomerId.HasValue)
                filtered = filtered.Where(l => l.CustomerId == request.CustomerId);

            if (request.StartDate.HasValue)
                filtered = filtered.Where(l => l.LaunchDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                filtered = filtered.Where(l => l.LaunchDate <= request.EndDate.Value);

            var totalItems = filtered.Count();
            var totalByCustomer = filtered.Sum(l => l.Amount);

            var pagedItems = filtered
                .OrderByDescending(l => l.LaunchDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList()
                .Select(l => new LaunchByClientResult
                {
                    CustomerName = l.Customer?.Name ?? "Sem cliente",
                    Description = l.Description,
                    Amount = l.Amount,
                    Type = l.Type,
                    Status = l.Status,
                    LaunchDate = l.LaunchDate
                }).ToList();

            return new PagedResultLaunchByClient
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalByCustomer = totalByCustomer,
                Items = pagedItems
            };
        }
    }
}
