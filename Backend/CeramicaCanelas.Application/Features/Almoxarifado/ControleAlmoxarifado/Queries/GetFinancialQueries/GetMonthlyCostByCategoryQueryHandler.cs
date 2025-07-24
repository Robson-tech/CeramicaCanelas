// No arquivo: GetMonthlyCostByCategoryQueryHandler.cs

// Usings necessários
using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.Common;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetFinancialQueries
{
    // A DEFINIÇÃO DA CLASSE "MonthlyCostReport" FOI REMOVIDA DAQUI

    public class GetMonthlyCostByCategoryQuery : IRequest<MonthlyCostReport>
    {
        // ... (o conteúdo da query não muda)
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? CategoryId { get; set; }
        public int? Year { get; set; }
    }

    public class GetMonthlyCostByCategoryQueryHandler : IRequestHandler<GetMonthlyCostByCategoryQuery, MonthlyCostReport>
    {
        // ... (o conteúdo do handler não muda)
        private readonly IProductRepository _productRepo;

        public GetMonthlyCostByCategoryQueryHandler(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<MonthlyCostReport> Handle(GetMonthlyCostByCategoryQuery request, CancellationToken cancellationToken)
        {
            // ... (toda a sua lógica de cálculo continua exatamente a mesma)

            var products = await _productRepo.GetAllProductsAsync();
            var query = products.AsQueryable();

            if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);

            if (request.Year.HasValue)
                query = query.Where(p => p.ModifiedOn.Year == request.Year.Value);

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

            var totalItems = groupedQuery.Count();
            var totalCostOverall = groupedQuery.Sum(g => g.TotalCost);
            var averageCostPerRecord = (totalItems > 0) ? totalCostOverall / totalItems : 0;

            var itemsForPage = groupedQuery
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var pagedData = new PagedResultDashboard<GetMonthlyCostByCategoryResult>(
                items: itemsForPage,
                totalItems: totalItems,
                page: request.Page,
                pageSize: request.PageSize
            );

            var finalReport = new MonthlyCostReport
            {
                PagedData = pagedData,
                TotalCostOverall = totalCostOverall,
                AverageCostPerRecord = averageCostPerRecord
            };

            return finalReport;
        }
    }
}