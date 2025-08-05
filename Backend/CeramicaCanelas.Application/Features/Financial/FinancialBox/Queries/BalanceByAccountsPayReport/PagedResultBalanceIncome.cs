using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.BalanceByAccountsPayReport
{
    public class PagedResultBalanceIncome
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        public decimal TotalIncomeOverall { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly
            ? EndDate { get; set; }


        public List<BalanceIncomeResult> Items { get; set; } = new();
    }
}
