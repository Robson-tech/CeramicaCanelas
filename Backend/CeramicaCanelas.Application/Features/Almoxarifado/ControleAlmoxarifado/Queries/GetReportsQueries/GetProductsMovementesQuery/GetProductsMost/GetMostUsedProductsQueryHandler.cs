using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.Common; // Adicione este using
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetProductsMost
{
    // 2. Mude a assinatura do Handler para corresponder à Query
    public class GetMostUsedProductsQueryHandler : IRequestHandler<GetMostUsedProductsQuery, PagedResultDashboard<GetMostUsedProductsResult>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMovExitProductsRepository _exitRepository;

        public GetMostUsedProductsQueryHandler(IProductRepository productRepository, IMovExitProductsRepository exitRepository)
        {
            _productRepository = productRepository;
            _exitRepository = exitRepository;
        }

        public async Task<PagedResultDashboard<GetMostUsedProductsResult>> Handle(GetMostUsedProductsQuery request, CancellationToken cancellationToken)
        {
            var allProducts = await _productRepository.GetAllProductsAsync();
            var allExits = await _exitRepository.GetAllAsync();

            var filteredProducts = allProducts.AsEnumerable();

            // 3. Aplica os filtros iniciais sobre os produtos
            if (!string.IsNullOrWhiteSpace(request.Search))
                filteredProducts = filteredProducts.Where(p => p.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase));

            if (request.CategoryId.HasValue && request.CategoryId != Guid.Empty)
                filteredProducts = filteredProducts.Where(p => p.CategoryId == request.CategoryId.Value);

            // 4. Calcula os dados e aplica o filtro final
            var calculatedAndFilteredData = filteredProducts
                .Select(p =>
                {
                    var saidasDoProduto = allExits.Where(e => e.ProductId == p.Id).ToList();
                    return new GetMostUsedProductsResult
                    {
                        ProductName = p.Name,
                        Category = p.Category?.Name ?? "Sem categoria",
                        TotalSaidas = saidasDoProduto.Sum(e => e.Quantity),
                        UltimaRetirada = saidasDoProduto.OrderByDescending(e => e.ExitDate).FirstOrDefault()?.ExitDate,
                        EstoqueAtual = p.StockCurrent
                    };
                })
                .Where(r => r.TotalSaidas > 0); // Filtro final

            // 5. Conte o total de itens APÓS TODOS os filtros e cálculos
            var totalItems = calculatedAndFilteredData.Count();

            // 6. Aplique a ordenação e a paginação
            var itemsForPage = calculatedAndFilteredData
                .OrderByDescending(r => r.TotalSaidas)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // 7. Crie e retorne o objeto PagedResultDashboard preenchido
            var pagedResult = new PagedResultDashboard<GetMostUsedProductsResult>(
                items: itemsForPage,
                totalItems: totalItems,
                page: request.Page,
                pageSize: request.PageSize
            );

            return pagedResult;
        }
    }
}