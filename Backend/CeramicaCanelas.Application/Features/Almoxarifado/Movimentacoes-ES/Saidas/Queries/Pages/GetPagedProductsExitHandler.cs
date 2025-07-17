using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.Movimentacoes_ES.Saidas.Queries.Pages
{
    public class GetPagedProductsExitHandler : IRequestHandler<PagedRequestProductsExit, PagedResultProductsExit>
    {
        private readonly IMovExitProductsRepository _exitRepository;

        public GetPagedProductsExitHandler(IMovExitProductsRepository exitRepository)
        {
            _exitRepository = exitRepository;
        }

        public async Task<PagedResultProductsExit> Handle(PagedRequestProductsExit request, CancellationToken cancellationToken)
        {
            var all = await _exitRepository.GetAllAsync();

            // Filtro por nome de produto ou funcionário
            if (!string.IsNullOrWhiteSpace(request.Search))
                all = all.Where(e =>
                    (e.Product?.Name?.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.Employee?.Name?.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();

            // Filtro por categoria
            if (request.CategoryId.HasValue && request.CategoryId != Guid.Empty)
                all = all.Where(e => e.Product?.CategoryId == request.CategoryId).ToList();

            // Ordenação
            all = request.OrderBy?.ToLower() switch
            {
                "productname" => request.Ascending ? all.OrderBy(e => e.Product?.Name).ToList() : all.OrderByDescending(e => e.Product?.Name).ToList(),
                "date" => request.Ascending ? all.OrderBy(e => e.ExitDate).ToList() : all.OrderByDescending(e => e.ExitDate).ToList(),
                _ => all.OrderByDescending(e => e.ExitDate).ToList()
            };

            var totalItems = all.Count;

            var pagedItems = all
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(exit => new ExitItemResult(exit))
                .ToList();

            return new PagedResultProductsExit
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                Items = pagedItems
            };
        }
    }
}
