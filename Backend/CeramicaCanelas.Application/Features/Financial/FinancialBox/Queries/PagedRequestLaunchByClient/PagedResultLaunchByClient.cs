using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PagedRequestLaunchByClient
{
    public class PagedResultLaunchByClient
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public decimal TotalByCustomer { get; set; } // soma dos lançamentos do cliente
        public List<LaunchByClientResult> Items { get; set; } = new();
    }
}
