namespace CeramicaCanelas.Application.Features.Almoxarifado.Movimentacoes_ES.Entradas.Queries.Pages
{
    public class PagedResultProductsEntry
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public List<EntryItemResult> Items { get; set; } = new();
    }
}
