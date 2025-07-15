using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using MediatR;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetProductsMost
{
    public class GetMostUsedProductsQueryHandler : IRequestHandler<GetMostUsedProductsQuery, List<GetMostUsedProductsResult>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMovExitProductsRepository _exitRepository;

        public GetMostUsedProductsQueryHandler(IProductRepository productRepository, IMovExitProductsRepository exitRepository)
        {
            _productRepository = productRepository;
            _exitRepository = exitRepository;
        }

        public async Task<List<GetMostUsedProductsResult>> Handle(GetMostUsedProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllProductsAsync();
            var exits = await _exitRepository.GetAllAsync();

            // Aplica os filtros opcionais
            if (!string.IsNullOrWhiteSpace(request.Search))
                products = products.Where(p => p.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase)).ToList();

            if (request.CategoryId.HasValue && request.CategoryId != Guid.Empty)
                products = products.Where(p => p.CategoryId == request.CategoryId.Value).ToList();

            // Calcula os dados
            var data = products
                .Select(p =>
                {
                    var saidas = exits.Where(e => e.ProductId == p.Id).ToList();
                    return new GetMostUsedProductsResult
                    {
                        ProductName = p.Name,
                        Category = p.Category?.Name ?? "Sem categoria",
                        TotalSaidas = saidas.Sum(e => e.Quantity),
                        UltimaRetirada = saidas.OrderByDescending(e => e.ExitDate).FirstOrDefault()?.ExitDate,
                        EstoqueAtual = p.StockCurrent
                    };
                })
                .Where(r => r.TotalSaidas > 0) // só produtos com saídas
                .OrderByDescending(r => r.TotalSaidas)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return data;
        }
    }
}
