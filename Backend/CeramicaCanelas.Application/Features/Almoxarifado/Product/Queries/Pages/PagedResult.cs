namespace CeramicaCanelas.Application.Features.Almoxarifado.Product.Queries.Pages
{
    public class PagedResult<T>
    {
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        public List<T> Items { get; set; } = new();
    }

}
