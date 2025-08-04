using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.BalanceByAccountsPayReport
{
    public class PagedRequestBalanceIncome : IRequest<PagedResultBalanceIncome>
    {
        public string? Search { get; set; } // ex: BBJ, CASH
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
