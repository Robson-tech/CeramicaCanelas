using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetFinancialQueries
{
    public class GetMonthlyCostByCategoryQuery : IRequest<List<GetMonthlyCostByCategoryResult>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? CategoryId { get; set; }
        public int? Year { get; set; }
    }

    public class GetMonthlyCostByCategoryQueryHandler : IRequestHandler<GetMonthlyCostByCategoryQuery, List<GetMonthlyCostByCategoryResult>>
    {
        private readonly IProductRepository _productRepo;

        public GetMonthlyCostByCategoryQueryHandler(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<List<GetMonthlyCostByCategoryResult>> Handle(GetMonthlyCostByCategoryQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepo.GetAllProductsAsync();

            var query = products.AsQueryable();

            if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);

            if (request.Year.HasValue)
                query = query.Where(p => p.ModifiedOn.Year == request.Year.Value);

            var grouped = query
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
                })
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return grouped;
        }
    }
}
