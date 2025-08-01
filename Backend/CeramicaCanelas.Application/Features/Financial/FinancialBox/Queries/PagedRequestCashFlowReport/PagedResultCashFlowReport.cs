using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PagedRequestCashFlowReport
{
    public class PagedResultCashFlowReport
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public decimal TotalEntradas { get; set; }
        public decimal TotalSaidas { get; set; }
        public decimal Saldo => TotalEntradas - TotalSaidas;
        public List<CashFlowReportItem> Items { get; set; } = new();
    }
}
