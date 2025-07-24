using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.Common; // Adicione este using
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetEmployeeMovementsQuery
{
    // 2. Mude a assinatura do Handler para corresponder à Query
    public class GetEmployeeMovementsQueryHandler : IRequestHandler<GetEmployeeMovementsQuery, PagedResultDashboard<GetEmployeeMovementsResult>>
    {
        private readonly IMovExitProductsRepository _exitRepository;

        public GetEmployeeMovementsQueryHandler(IMovExitProductsRepository exitRepository)
        {
            _exitRepository = exitRepository;
        }

        public async Task<PagedResultDashboard<GetEmployeeMovementsResult>> Handle(GetEmployeeMovementsQuery request, CancellationToken cancellationToken)
        {
            var allMovements = await _exitRepository.GetAllAsync();
            var filteredData = allMovements.AsEnumerable(); // Usar AsEnumerable para aplicar filtros em memória

            // 3. Aplique todos os filtros primeiro
            if (!string.IsNullOrWhiteSpace(request.SearchEmployee))
                filteredData = filteredData.Where(x => x.EmployeeName.Contains(request.SearchEmployee, StringComparison.OrdinalIgnoreCase));

            if (request.StartDate.HasValue)
                filteredData = filteredData.Where(x => x.ExitDate.Date >= request.StartDate.Value.Date);

            if (request.EndDate.HasValue)
                filteredData = filteredData.Where(x => x.ExitDate.Date <= request.EndDate.Value.Date);

            // 4. Conte o total de itens APÓS os filtros
            var totalItems = filteredData.Count();

            // 5. Aplique a ordenação, paginação e o mapeamento para o DTO
            var itemsForPage = filteredData
                .OrderByDescending(x => x.ExitDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new GetEmployeeMovementsResult
                {
                    EmployeeName = x.EmployeeName,
                    ProductName = x.NameProduct,
                    QuantityRetirada = x.Quantity,
                    QuantityDevolvida = x.ReturnedQuantity,
                    QuantityPendente = x.IsReturnable ? x.Quantity - x.ReturnedQuantity : 0,
                    DataRetirada = x.ExitDate
                })
                .ToList();

            // 6. Crie e retorne o objeto PagedResultDashboard preenchido
            var pagedResult = new PagedResultDashboard<GetEmployeeMovementsResult>(
                items: itemsForPage,
                totalItems: totalItems,
                page: request.Page,
                pageSize: request.PageSize
            );

            return pagedResult;
        }
    }
}