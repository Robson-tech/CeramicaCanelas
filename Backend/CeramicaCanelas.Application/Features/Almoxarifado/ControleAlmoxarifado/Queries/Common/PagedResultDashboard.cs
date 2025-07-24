namespace CeramicaCanelas.Application.Features.Almoxarifado.ControleAlmoxarifado.Queries.Common
{
    public class PagedResultDashboard<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public List<T> Items { get; set; } = [];

        public PagedResultDashboard(List<T> items, int totalItems, int page, int pageSize)
        {
            Items = items;
            TotalItems = totalItems;
            Page = page;
            PageSize = pageSize;
        }

        // Construtor vazio para inicialização
        public PagedResultDashboard() { }
    }
}