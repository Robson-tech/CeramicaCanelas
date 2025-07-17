using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.Movimentacoes_ES.Saidas.Queries.Pages
{
    public class PagedRequestProductsExit : IRequest<PagedResultProductsExit>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; } // Busca por nome do produto ou funcionário
        public Guid? CategoryId { get; set; }
        public string? OrderBy { get; set; }
        public bool Ascending { get; set; } = true;
    }
}
