using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetUnreturnedProducts
{
    public class GetUnreturnedProductsQueryHandler : IRequestHandler<GetUnreturnedProductsQuery, List<GetUnreturnedProductsResult>>
    {
        private readonly IMovExitProductsRepository _exitRepository;

        public GetUnreturnedProductsQueryHandler(IMovExitProductsRepository exitRepository)
        {
            _exitRepository = exitRepository;
        }

        public async Task<List<GetUnreturnedProductsResult>> Handle(GetUnreturnedProductsQuery request, CancellationToken cancellationToken)
        {
            var exits = await _exitRepository.GetAllAsync();

            var filtered = exits
                .Where(e =>
                    e.IsReturnable &&
                    e.Quantity > e.ReturnedQuantity &&
                    (string.IsNullOrEmpty(request.Search) || e.Product.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase)) &&
                    (!request.CategoryId.HasValue || e.Product.CategoryId == request.CategoryId.Value) &&
                    (string.IsNullOrEmpty(request.EmployeeName) || e.EmployeeName.Contains(request.EmployeeName, StringComparison.OrdinalIgnoreCase)) &&
                    (!request.StartDate.HasValue || e.ExitDate.Date >= request.StartDate.Value.Date) &&
                    (!request.EndDate.HasValue || e.ExitDate.Date <= request.EndDate.Value.Date)
                )
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

            return filtered;
        }

    }
}
