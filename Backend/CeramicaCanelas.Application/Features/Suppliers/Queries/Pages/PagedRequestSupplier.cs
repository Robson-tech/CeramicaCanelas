using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Suppliers.Queries.Pages
{
    public class PagedRequestSupplier : IRequest<PagedResultSupplier>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; } // Filtro por nome
        public string? OrderBy { get; set; } // Ex: "name", "email"
        public bool Ascending { get; set; } = true;
    }
}
