using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using MediatR;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetEmployeeMovementsQuery
{
    public class GetEmployeeMovementsQueryHandler : IRequestHandler<GetEmployeeMovementsQuery, List<GetEmployeeMovementsResult>>
    {
        private readonly IMovExitProductsRepository _exitRepository;

        public GetEmployeeMovementsQueryHandler(IMovExitProductsRepository exitRepository)
        {
            _exitRepository = exitRepository;
        }

        public async Task<List<GetEmployeeMovementsResult>> Handle(GetEmployeeMovementsQuery request, CancellationToken cancellationToken)
        {
            var data = await _exitRepository.GetAllAsync();

            // Filtro por nome
            if (!string.IsNullOrWhiteSpace(request.SearchEmployee))
                data = data.Where(x => x.Employee.Name.Contains(request.SearchEmployee)).ToList();

            // Filtro por data
            if (request.StartDate.HasValue)
                data = data.Where(x => x.ExitDate >= request.StartDate.Value).ToList();
            if (request.EndDate.HasValue)
                data = data.Where(x => x.ExitDate <= request.EndDate.Value).ToList();

            var result = data
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
                }).ToList();


            return result;
        }
    }

}
