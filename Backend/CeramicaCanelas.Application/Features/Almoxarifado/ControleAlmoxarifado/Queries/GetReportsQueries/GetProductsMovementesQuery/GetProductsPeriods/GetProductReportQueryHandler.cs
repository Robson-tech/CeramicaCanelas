using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.Common; // Adicione este using
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetProductsPeriods
{
    // 2. Mude a assinatura do Handler para corresponder à Query
    public class GetProductReportQueryHandler : IRequestHandler<GetProductReportQuery, PagedResultDashboard<GetProductReportResult>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMovEntryProductsRepository _entryRepository;
        private readonly IMovExitProductsRepository _exitRepository;

        public GetProductReportQueryHandler(
            IProductRepository productRepository,
            IMovEntryProductsRepository entryRepository,
            IMovExitProductsRepository exitRepository)
        {
            _productRepository = productRepository;
            _entryRepository = entryRepository;
            _exitRepository = exitRepository;
        }

        public async Task<PagedResultDashboard<GetProductReportResult>> Handle(GetProductReportQuery request, CancellationToken cancellationToken)
        {
            // 3. PRIMEIRA CHAMADA: Obtenha a contagem total de produtos com os filtros
            var totalItems = await _productRepository.GetTotalCountAsync(
                search: request.SearchProduct,
                minPrice: null,
                maxPrice: null,
                categoryId: request.CategoryId
            );

            // 4. SEGUNDA CHAMADA: Obtenha os produtos da página atual
            var productsForPage = await _productRepository.GetPagedAsync(
                request.Page,
                request.PageSize,
                orderBy: "name",
                ascending: true,
                search: request.SearchProduct,
                minPrice: null,
                maxPrice: null,
                categoryId: request.CategoryId
            );

            // A lógica de busca e filtro de movimentações continua a mesma
            var allEntries = await _entryRepository.GetAllAsync();
            var allExits = await _exitRepository.GetAllAsync();

            if (request.StartDate.HasValue)
            {
                allEntries = allEntries.Where(e => e.EntryDate.Date >= request.StartDate.Value.Date).ToList();
                allExits = allExits.Where(e => e.ExitDate.Date >= request.StartDate.Value.Date).ToList();
            }
            if (request.EndDate.HasValue)
            {
                allEntries = allEntries.Where(e => e.EntryDate.Date <= request.EndDate.Value.Date).ToList();
                allExits = allExits.Where(e => e.ExitDate.Date <= request.EndDate.Value.Date).ToList();
            }

            // 5. Mapeie os itens da página para o DTO de resultado
            var items = productsForPage.Select(product =>
            {
                var entries = allEntries.Where(e => e.ProductId == product.Id);
                var exits = allExits.Where(e => e.ProductId == product.Id);

                var totalEntradas = entries.Sum(e => e.Quantity);
                var totalSaidas = exits.Sum(e => e.Quantity);

                var totalPendentes = exits
                    .Where(e => e.IsReturnable)
                    .Sum(e => e.Quantity - e.ReturnedQuantity);

                var ultimaMovimentacao = new[]
                {
                    entries.OrderByDescending(e => e.EntryDate).FirstOrDefault()?.EntryDate,
                    exits.OrderByDescending(e => e.ExitDate).FirstOrDefault()?.ExitDate
                }
                .Where(d => d.HasValue)
                .Max();

                string status = product.StockCurrent <= 0
                    ? "Em Falta"
                    : product.StockCurrent < product.StockMinium ? "Abaixo do Mínimo" : "Normal";

                return new GetProductReportResult
                {
                    ProductName = product.Name,
                    CategoryName = product.Category?.Name ?? "Categoria não encontrada",
                    StockCurrent = product.StockCurrent,
                    StockMinimum = product.StockMinium,
                    TotalEntries = totalEntradas,
                    TotalExits = totalSaidas,
                    PendingReturns = totalPendentes,
                    StockStatus = status,
                    LastMovementDate = ultimaMovimentacao
                };
            }).ToList();

            // 6. Crie e retorne o objeto PagedResultDashboard
            var pagedResult = new PagedResultDashboard<GetProductReportResult>(
                items: items,
                totalItems: totalItems,
                page: request.Page,
                pageSize: request.PageSize
            );

            return pagedResult;
        }
    }
}