using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PagedRequestLaunchByClient
{
    public class PagedClientIncomeResult
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; } // Total de clientes, não de lançamentos
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public List<ClientIncomeSummaryResult> Items { get; set; } = new();
    }
}
