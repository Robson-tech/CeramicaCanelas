using CeramicaCanelas.Domain.Enums;
using MediatR;


namespace CeramicaCanelas.Application.Features.Employees.Queries.Pages
{
    public class PagedRequestEmployees : IRequest<PagedResultEmployees>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; } // Nome do funcionário
        public Positiions? Positions { get; set; }   // Filtro por função/cargo
        public bool? ActiveOnly { get; set; } // Ativos apenas?
        public string? OrderBy { get; set; }
        public bool Ascending { get; set; } = true;
    }
}
