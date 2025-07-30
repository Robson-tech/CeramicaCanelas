using CeramicaCanelas.Domain.Abstract;
using CeramicaCanelas.Domain.Entities.Almoxarifado;


namespace CeramicaCanelas.Domain.Entities
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Cnpj { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        public ICollection<ProductEntry> ProductEntries { get; set; } = new List<ProductEntry>();
    }
}
