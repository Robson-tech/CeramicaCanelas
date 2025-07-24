// Adicione os usings necessários para as classes de DTO e Paginação
using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.Common;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetInventoryStatusQueries
{
    // 1. Mude o tipo de retorno da Query para o seu DTO de paginação
    public class GetInventoryStatusQuery : IRequest<PagedResultDashboard<GetInventoryStatusResult>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public Guid? CategoryId { get; set; }
    }

    // 2. Mude a assinatura do Handler para corresponder à Query
    public class GetInventoryStatusQueryHandler : IRequestHandler<GetInventoryStatusQuery, PagedResultDashboard<GetInventoryStatusResult>>
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

        public async Task<PagedResultDashboard<GetInventoryStatusResult>> Handle(GetInventoryStatusQuery request, CancellationToken cancellationToken)
        {
            // 3. PRIMEIRA CHAMADA: Obtenha o número total de itens usando os mesmos filtros
            var totalItems = await _productRepository.GetTotalCountAsync(
                search: request.Search,
                minPrice: null,
                maxPrice: null,
                categoryId: request.CategoryId
            );

            // 4. SEGUNDA CHAMADA: Obtenha os itens da página atual
            var productsForPage = await _productRepository.GetPagedAsync(
                page: request.Page,
                pageSize: request.PageSize,
                orderBy: "name",
                ascending: true,
                search: request.Search,
                minPrice: null,
                maxPrice: null,
                categoryId: request.CategoryId
            );

            // A lógica de busca de todas as entradas/saídas continua (com a nota de performance em mente)
            var entries = await _entryRepository.GetAllAsync();
            var exits = await _exitRepository.GetAllAsync();

            // 5. Mapeie os itens da página para o seu DTO de resultado
            var items = productsForPage.Select(p =>
            {
                var totalEntradas = entries.Where(e => e.ProductId == p.Id).Sum(e => e.Quantity);
                var totalSaidas = exits.Where(e => e.ProductId == p.Id).Sum(e => e.Quantity);
                var ultimaEntrada = entries.Where(e => e.ProductId == p.Id).OrderByDescending(e => e.EntryDate).FirstOrDefault()?.EntryDate;
                var ultimaSaida = exits.Where(e => e.ProductId == p.Id).OrderByDescending(e => e.ExitDate).FirstOrDefault()?.ExitDate;
                DateTime? ultimaMovimentacao = ultimaEntrada > ultimaSaida ? ultimaEntrada : ultimaSaida ?? ultimaEntrada ?? ultimaSaida;
                string status = p.StockCurrent <= 0 ? "Em Falta" : (p.StockCurrent < p.StockMinium ? "Abaixo do Mínimo" : "Normal");

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

            // 6. Crie e retorne o objeto PagedResultDashboard preenchido com os resultados das duas chamadas
            var pagedResult = new PagedResultDashboard<GetInventoryStatusResult>(
                items: items,
                totalItems: totalItems,
                page: request.Page,
                pageSize: request.PageSize
            );

            return pagedResult;
        }
    }
}