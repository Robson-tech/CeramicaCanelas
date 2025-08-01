using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Customers.Queries.GetPagedCustomerQueries
{
    public class PagedRequestCustomer : IRequest<PagedResultCustomer>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; } // Busca por nome, email, etc.
        public string? OrderBy { get; set; } // name, email, etc.
        public bool Ascending { get; set; } = true;
    }
}
