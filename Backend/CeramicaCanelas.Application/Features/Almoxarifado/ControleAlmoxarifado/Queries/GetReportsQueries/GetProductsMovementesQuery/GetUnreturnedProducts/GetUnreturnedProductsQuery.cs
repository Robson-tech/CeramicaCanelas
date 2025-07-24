using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.Common; // Adicione este using
using MediatR;
using System;
using System.Collections.Generic;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetUnreturnedProducts
{
    // 1. Mude o tipo de retorno da Query
    public class GetUnreturnedProductsQuery : IRequest<PagedResultDashboard<GetUnreturnedProductsResult>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public Guid? CategoryId { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}