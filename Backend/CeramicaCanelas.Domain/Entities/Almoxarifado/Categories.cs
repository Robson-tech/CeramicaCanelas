using CeramicaCanelas.Domain.Abstract;
using System.Text.Json.Serialization;


namespace CeramicaCanelas.Domain.Entities.Almoxarifado
{
    public class Categories : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();

    }
}
