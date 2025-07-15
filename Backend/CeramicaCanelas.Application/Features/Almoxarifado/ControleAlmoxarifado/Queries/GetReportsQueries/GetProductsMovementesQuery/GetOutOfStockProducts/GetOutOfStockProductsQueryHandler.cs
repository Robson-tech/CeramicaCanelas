using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetOutOfStockProducts
{
    public class GetOutOfStockProductsQueryHandler : IRequestHandler<GetOutOfStockProductsQuery, List<GetOutOfStockProductsResult>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMovExitProductsRepository _exitRepository;

        public GetOutOfStockProductsQueryHandler(IProductRepository productRepository, IMovExitProductsRepository exitRepository)
        {
            _productRepository = productRepository;
            _exitRepository = exitRepository;
        }

        public async Task<List<GetOutOfStockProductsResult>> Handle(GetOutOfStockProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllProductsAsync();
            var exits = await _exitRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(request.Search))
                products = products.Where(p => p.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase)).ToList();

            if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
                products = products.Where(p => p.CategoryId == request.CategoryId.Value).ToList();

            var data = products
                .Where(p => p.StockCurrent <= 0)
                .Select(p =>
                {
                    var ultimaSaida = exits
                        .Where(e => e.ProductId == p.Id)
                        .OrderByDescending(e => e.ExitDate)
                        .FirstOrDefault()?.ExitDate;

                    return new GetOutOfStockProductsResult
                    {
                        ProductName = p.Name,
                        Category = p.Category?.Name ?? "Sem categoria",
                        StockCurrent = p.StockCurrent,
                        StockMinimum = p.StockMinium,
                        LastExit = ultimaSaida,
                        Status = "Em Falta"
                    };
                })
                .OrderBy(p => p.ProductName)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return data;
        }
    }
}
