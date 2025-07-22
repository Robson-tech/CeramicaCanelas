using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetProductsPeriods
{

    public class GetProductReportQueryHandler : IRequestHandler<GetProductReportQuery, List<GetProductReportResult>>
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

        public async Task<List<GetProductReportResult>> Handle(GetProductReportQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetPagedAsync(
                request.Page,
                request.PageSize,
                orderBy: "name",
                ascending: true,
                search: request.SearchProduct,
                minPrice: null,
                maxPrice: null,
                categoryId: request.CategoryId
            );

            var allEntries = await _entryRepository.GetAllAsync();
            var allExits = await _exitRepository.GetAllAsync();

            if (request.StartDate.HasValue)
            {
                allEntries = allEntries.Where(e => e.EntryDate >= request.StartDate.Value).ToList();
                allExits = allExits.Where(e => e.ExitDate >= request.StartDate.Value).ToList();
            }
            if (request.EndDate.HasValue)
            {
                allEntries = allEntries.Where(e => e.EntryDate <= request.EndDate.Value).ToList();
                allExits = allExits.Where(e => e.ExitDate <= request.EndDate.Value).ToList();
            }

            var result = products.Select(product =>
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

            return result;
        }
    }


}
