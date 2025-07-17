using MediatR;
using System;

namespace CeramicaCanelas.Application.Features.Almoxarifado.Movimentacoes_ES.Entradas.Queries.Pages
{
    public class PagedRequestProductsEntry : IRequest<PagedResultProductsEntry>
    {
        public int Page { get; set; } = 1;                     // Página atual
        public int PageSize { get; set; } = 10;                // Tamanho da página
        public string? Search { get; set; }                    // Nome do produto
        public Guid? CategoryId { get; set; }                  // Filtro por categoria
        public string? OrderBy { get; set; }                   // Campo de ordenação (ex: "productname", "date")
        public bool Ascending { get; set; } = true;            // Direção da ordenação (true = crescente)
    }
}
