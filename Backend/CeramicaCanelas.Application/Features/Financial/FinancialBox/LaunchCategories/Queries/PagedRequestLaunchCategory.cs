using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.LaunchCategories.Queries
{
    public class PagedRequestLaunchCategory : IRequest<PagedResultLaunchCategory>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; } // Buscar por nome
        public string? OrderBy { get; set; } // "name"
        public bool Ascending { get; set; } = true;
    }
}
