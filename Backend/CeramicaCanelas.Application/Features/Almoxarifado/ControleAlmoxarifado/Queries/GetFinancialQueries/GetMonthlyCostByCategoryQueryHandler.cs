using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.Common; // Adicione este using!
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Application.Features.Almoxarifado.Product.Queries.Pages;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetFinancialQueries
{
    // 1. Mude o tipo de retorno da Query
    public class GetMonthlyCostByCategoryQuery : IRequest<PagedResultDashboard<GetMonthlyCostByCategoryResult>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? CategoryId { get; set; }
        public int? Year { get; set; }
    }

    // 2. Mude a assinatura do Handler para corresponder à Query
    public class GetMonthlyCostByCategoryQueryHandler : IRequestHandler<GetMonthlyCostByCategoryQuery, PagedResultDashboard<GetMonthlyCostByCategoryResult>>
    {
        private readonly IProductRepository _productRepo;

        public GetMonthlyCostByCategoryQueryHandler(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<PagedResultDashboard<GetMonthlyCostByCategoryResult>> Handle(GetMonthlyCostByCategoryQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepo.GetAllProductsAsync();

            var query = products.AsQueryable();

            if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);

            if (request.Year.HasValue)
                query = query.Where(p => p.ModifiedOn.Year == request.Year.Value);

            // Crie a consulta base com o agrupamento
            var groupedQuery = query
                .GroupBy(p => new
                {
                    CategoryName = p.Category.Name,
                    Year = p.ModifiedOn.Year,
                    Month = p.ModifiedOn.Month
                })
                .Select(g => new GetMonthlyCostByCategoryResult
                {
                    CategoryName = g.Key.CategoryName,
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalCost = g.Sum(p => p.ValueTotal)
                });

            // 3. Calcule o total de itens ANTES de paginar
            var totalItems = groupedQuery.Count();

            // 4. Aplique a ordenação e a paginação para obter os itens da página atual
            var itemsForPage = groupedQuery
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // 5. Crie e retorne o objeto PagedResult preenchido
            var pagedResult = new PagedResultDashboard<GetMonthlyCostByCategoryResult>(
                items: itemsForPage,
                totalItems: totalItems,
                page: request.Page,
                pageSize: request.PageSize
            );

            return pagedResult;
        }
    }
}