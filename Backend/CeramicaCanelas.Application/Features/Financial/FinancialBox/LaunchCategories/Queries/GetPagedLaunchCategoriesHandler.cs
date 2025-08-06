using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.LaunchCategories.Queries
{
    public class GetPagedLaunchCategoriesHandler : IRequestHandler<PagedRequestLaunchCategory, PagedResultLaunchCategory>
    {
        private readonly ILaunchCategoryRepository _categoryRepository;

        public GetPagedLaunchCategoriesHandler(ILaunchCategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<PagedResultLaunchCategory> Handle(PagedRequestLaunchCategory request, CancellationToken cancellationToken)
        {
            var all = _categoryRepository.QueryAllWithIncludes();

            var filtered = all.Where(c => !c.IsDeleted);

            // Filtro por nome
            // Filtro por nome (corrigido para compatibilidade com EF Core)
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchLower = request.Search.ToLower();
                filtered = filtered.Where(c => c.Name.ToLower().Contains(searchLower));
            }

            // Ordenação
            filtered = request.OrderBy?.ToLower() switch
            {
                "name" => request.Ascending ? filtered.OrderBy(c => c.Name) : filtered.OrderByDescending(c => c.Name),
                _ => filtered.OrderBy(c => c.Name)
            };

            var totalItems = filtered.Count();

            var pagedItems = filtered
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new LaunchCategoryResult(c))
                .ToList();

            return new PagedResultLaunchCategory
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                Items = pagedItems
            };
        }
    }
}
