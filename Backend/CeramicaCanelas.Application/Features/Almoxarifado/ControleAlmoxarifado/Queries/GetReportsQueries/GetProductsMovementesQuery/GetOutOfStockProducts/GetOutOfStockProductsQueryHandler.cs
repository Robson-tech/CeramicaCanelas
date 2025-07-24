using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.Common; // Adicione este using
using MediatR;


namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetOutOfStockProducts
{
    // 2. Mude a assinatura do Handler para corresponder à Query
    public class GetOutOfStockProductsQueryHandler : IRequestHandler<GetOutOfStockProductsQuery, PagedResultDashboard<GetOutOfStockProductsResult>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMovExitProductsRepository _exitRepository;

        public GetOutOfStockProductsQueryHandler(IProductRepository productRepository, IMovExitProductsRepository exitRepository)
        {
            _productRepository = productRepository;
            _exitRepository = exitRepository;
        }

        public async Task<PagedResultDashboard<GetOutOfStockProductsResult>> Handle(GetOutOfStockProductsQuery request, CancellationToken cancellationToken)
        {
            var allProducts = await _productRepository.GetAllProductsAsync();
            var allExits = await _exitRepository.GetAllAsync();

            var filteredProducts = allProducts.AsEnumerable();

            // 3. Aplique todos os filtros primeiro
            if (!string.IsNullOrWhiteSpace(request.Search))
                filteredProducts = filteredProducts.Where(p => p.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase));

            if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
                filteredProducts = filteredProducts.Where(p => p.CategoryId == request.CategoryId.Value);

            // Filtro principal da query: produtos sem estoque
            filteredProducts = filteredProducts.Where(p => p.StockCurrent <= 0);

            // 4. Conte o total de itens APÓS os filtros
            var totalItems = filteredProducts.Count();

            // 5. Aplique a ordenação, paginação e o mapeamento para o DTO
            var itemsForPage = filteredProducts
                .OrderBy(p => p.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p =>
                {
                    // A busca pela última saída continua sendo feita aqui, na lista menor de itens paginados
                    var ultimaSaida = allExits
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
                .ToList();

            // 6. Crie e retorne o objeto PagedResultDashboard preenchido
            var pagedResult = new PagedResultDashboard<GetOutOfStockProductsResult>(
                items: itemsForPage,
                totalItems: totalItems,
                page: request.Page,
                pageSize: request.PageSize
            );

            return pagedResult;
        }
    }
}