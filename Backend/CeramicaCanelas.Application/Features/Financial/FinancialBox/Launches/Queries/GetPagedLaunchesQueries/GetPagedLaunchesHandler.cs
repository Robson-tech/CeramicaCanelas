using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Entities.Financial;
using MediatR;
using System.Linq;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Launches.Queries.GetPagedLaunchesQueries
{
    public class GetPagedLaunchesHandler : IRequestHandler<PagedRequestLaunch, PagedResultLaunch>
    {
        private readonly ILaunchRepository _launchRepository;

        public GetPagedLaunchesHandler(ILaunchRepository launchRepository)
        {
            _launchRepository = launchRepository;
        }

        public async Task<PagedResultLaunch> Handle(PagedRequestLaunch request, CancellationToken cancellationToken)
        {
            IEnumerable<Launch> launches = _launchRepository.QueryAllWithIncludes();

            // Filtro por descrição
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                launches = launches.Where(l => l.Description.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.SearchCategoryOrCustomer))
            {
                // Filtro por categoria ou cliente
                launches = launches.Where(l =>
                    (l.Category != null && l.Category.Name.Contains(request.SearchCategoryOrCustomer, StringComparison.OrdinalIgnoreCase)) ||
                    (l.Customer != null && l.Customer.Name.Contains(request.SearchCategoryOrCustomer, StringComparison.OrdinalIgnoreCase)));
            }

            // Filtro por tipo (Entrada/Saída)
            if (request.Type.HasValue)
            {
                launches = launches.Where(l => l.Type == request.Type.Value);
            }

            // Filtro por intervalo de datas
            if (request.StartDate.HasValue)
            {
                launches = launches.Where(l => l.LaunchDate >= request.StartDate.Value);
            }
            if (request.EndDate.HasValue)
            {
                launches = launches.Where(l => l.LaunchDate <= request.EndDate.Value);
            }

            // Filtro por categoria
            if (request.CategoryId.HasValue)
            {
                launches = launches.Where(l => l.CategoryId == request.CategoryId.Value);
            }

            // Filtro por cliente
            if (request.CustomerId.HasValue)
            {
                launches = launches.Where(l => l.CustomerId == request.CustomerId.Value);
            }

            // Filtro por status
            if (request.Status.HasValue)
            {
                launches = launches.Where(l => l.Status == request.Status.Value);
            }

            var totalItems = launches.Count();

            // Paginação (padrão: página 1, 10 itens por página)
            var pagedItems = launches
                .OrderByDescending(l => l.LaunchDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(l => new LaunchResult(l))
                .ToList();

            return new PagedResultLaunch
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                Items = pagedItems
            };
        }
    }
}
