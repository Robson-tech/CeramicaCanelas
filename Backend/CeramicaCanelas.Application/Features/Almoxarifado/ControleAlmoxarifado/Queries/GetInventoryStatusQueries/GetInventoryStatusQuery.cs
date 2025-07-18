using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetInventoryStatusQueries
{
    public class GetInventoryStatusQuery : IRequest<List<GetInventoryStatusResult>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }           // Busca pelo nome do produto
        public Guid? CategoryId { get; set; }         // Filtro por categoria
    }

    public class GetInventoryStatusQueryHandler : IRequestHandler<GetInventoryStatusQuery, List<GetInventoryStatusResult>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMovEntryProductsRepository _entryRepository;
        private readonly IMovExitProductsRepository _exitRepository;

        public GetInventoryStatusQueryHandler(
            IProductRepository productRepository,
            IMovEntryProductsRepository entryRepository,
            IMovExitProductsRepository exitRepository)
        {
            _productRepository = productRepository;
            _entryRepository = entryRepository;
            _exitRepository = exitRepository;
        }

        public async Task<List<GetInventoryStatusResult>> Handle(GetInventoryStatusQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository
                .GetPagedAsync(
                    request.Page,
                    request.PageSize,
                    orderBy: "name",
                    ascending: true,
                    search: request.Search,
                    minPrice: null,
                    maxPrice: null,
                    categoryId: request.CategoryId
                );

            var entries = await _entryRepository.GetAllAsync();
            var exits = await _exitRepository.GetAllAsync();

            var result = products.Select(p =>
            {
                var totalEntradas = entries.Where(e => e.ProductId == p.Id).Sum(e => e.Quantity);
                var totalSaidas = exits.Where(e => e.ProductId == p.Id).Sum(e => e.Quantity);

                var ultimaEntrada = entries.Where(e => e.ProductId == p.Id).OrderByDescending(e => e.EntryDate).FirstOrDefault()?.EntryDate;
                var ultimaSaida = exits.Where(e => e.ProductId == p.Id).OrderByDescending(e => e.ExitDate).FirstOrDefault()?.ExitDate;

                DateTime? ultimaMovimentacao = ultimaEntrada > ultimaSaida ? ultimaEntrada : ultimaSaida ?? ultimaEntrada ?? ultimaSaida;

                string status = p.StockCurrent <= 0
                    ? "Em Falta"
                    : (p.StockCurrent < p.StockMinium ? "Abaixo do Mínimo" : "Normal");

                return new GetInventoryStatusResult
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    StockInitial = p.StockInitial,
                    TotalEntries = totalEntradas,
                    TotalExits = totalSaidas,
                    StockCurrent = p.StockCurrent,
                    StockMinimum = p.StockMinium,
                    StockStatus = status,
                    Category = p.Category.Name,
                    LastMovement = ultimaMovimentacao
                  
                };
            }).ToList();

            return result;
        }

    }
}
