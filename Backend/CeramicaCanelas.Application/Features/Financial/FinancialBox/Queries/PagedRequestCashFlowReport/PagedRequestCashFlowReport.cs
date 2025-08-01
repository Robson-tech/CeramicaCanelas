using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PagedRequestCashFlowReport
{
    public class PagedRequestCashFlowReport : IRequest<PagedResultCashFlowReport>
    {
        public LaunchType type { get; set; } // Tipo de lançamento (Entrada, Saída ou Todos)
        public DateTime? StartDate { get; set; } // Período inicial
        public DateTime? EndDate { get; set; }   // Período final
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
