using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.Common; // Adicione este using
using MediatR;
using System;
using System.Collections.Generic;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetProductsMovementesQuery.GetOutOfStockProducts
{
    // 1. Mude o tipo de retorno da Query
    public class GetOutOfStockProductsQuery : IRequest<PagedResultDashboard<GetOutOfStockProductsResult>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public Guid? CategoryId { get; set; }
    }
}