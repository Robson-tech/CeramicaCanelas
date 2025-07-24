using CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.Common; // Adicione este using
using MediatR;
using System;
using System.Collections.Generic;

namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.GetReportsQueries.GetEmployeeMovementsQuery
{
    // 1. Mude o tipo de retorno da Query
    public class GetEmployeeMovementsQuery : IRequest<PagedResultDashboard<GetEmployeeMovementsResult>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchEmployee { get; set; } // busca por nome
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}