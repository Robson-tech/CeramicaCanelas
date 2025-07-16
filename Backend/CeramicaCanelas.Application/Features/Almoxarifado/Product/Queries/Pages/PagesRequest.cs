using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.Product.Queries.Pages
{
    public class PagedRequest : IRequest<PagedResult<GetAllProductsQueriesResult>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? OrderBy { get; set; }
        public bool Ascending { get; set; } = true;
        public string? Search { get; set; }
        public float? MinPrice { get; set; }
        public float? MaxPrice { get; set; }
        public Guid CategoryId { get; set; }
    }
}
