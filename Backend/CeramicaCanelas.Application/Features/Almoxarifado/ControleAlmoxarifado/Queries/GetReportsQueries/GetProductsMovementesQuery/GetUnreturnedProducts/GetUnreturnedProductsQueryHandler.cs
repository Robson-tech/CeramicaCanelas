using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.Common; // Adicione este using
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetUnreturnedProducts
{
    // 2. Mude a assinatura do Handler para corresponder à Query
    public class GetUnreturnedProductsQueryHandler : IRequestHandler<GetUnreturnedProductsQuery, PagedResultDashboard<GetUnreturnedProductsResult>>
    {
        private readonly IMovExitProductsRepository _exitRepository;

        public GetUnreturnedProductsQueryHandler(IMovExitProductsRepository exitRepository)
        {
            _exitRepository = exitRepository;
        }

        public async Task<PagedResultDashboard<GetUnreturnedProductsResult>> Handle(GetUnreturnedProductsQuery request, CancellationToken cancellationToken)
        {
            var allExits = await _exitRepository.GetAllAsync();

            // 3. Aplique todos os filtros para criar a consulta base
            var filteredData = allExits
                .Where(e =>
                    e.IsReturnable &&
                    e.Quantity > e.ReturnedQuantity &&
                    (string.IsNullOrEmpty(request.Search) || e.NameProduct.Contains(request.Search, StringComparison.OrdinalIgnoreCase)) &&
                    (!request.CategoryId.HasValue || e.Product!.CategoryId == request.CategoryId.Value) &&
                    (string.IsNullOrEmpty(request.EmployeeName) || e.EmployeeName.Contains(request.EmployeeName, StringComparison.OrdinalIgnoreCase)) &&
                    (!request.StartDate.HasValue || e.ExitDate.Date >= request.StartDate.Value.Date) &&
                    (!request.EndDate.HasValue || e.ExitDate.Date <= request.EndDate.Value.Date)
                );

            // 4. Conte o total de itens a partir dos dados já filtrados
            var totalItems = filteredData.Count();

            // 5. Aplique a ordenação, paginação e o mapeamento para o DTO
            var itemsForPage = filteredData
                .OrderByDescending(e => e.ExitDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new GetUnreturnedProductsResult
                {
                    Id = e.Id,
                    ProductName = e.NameProduct,
                    EmployeeName = e.EmployeeName,
                    QuantityRetirada = e.Quantity,
                    QuantityDevolvida = e.ReturnedQuantity,
                    QuantityPendente = e.Quantity - e.ReturnedQuantity,
                    DataRetirada = e.ExitDate,
                })
                .ToList();

            // 6. Crie e retorne o objeto PagedResultDashboard
            var pagedResult = new PagedResultDashboard<GetUnreturnedProductsResult>(
                items: itemsForPage,
                totalItems: totalItems,
                page: request.Page,
                pageSize: request.PageSize
            );

            return pagedResult;
        }
    }
}