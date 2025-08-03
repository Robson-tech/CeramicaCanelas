using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PagedRequestLaunchByClient
{
    public class PagedClientIncomeRequest : IRequest<PagedClientIncomeResult>
    {
        // Filtros
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Search { get; set; } // Para buscar por nome de cliente

        // Paginação
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string OrderBy { get; set; } = "name"; // Padrão de ordenação
        public bool Ascending { get; set; } = true;
    }
}
