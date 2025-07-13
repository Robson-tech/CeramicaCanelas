using CeramicaCanelas.Domain.Abstract;


namespace CeramicaCanelas.Domain.Entities
{
    public class Categories : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        // Navigation properties
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();

    }
}
