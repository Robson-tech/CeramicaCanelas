using CeramicaCanelas.Domain.Abstract;
using CeramicaCanelas.Domain.Enums;

namespace CeramicaCanelas.Domain.Entities
{
    public class Employee : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public Positiions Positiions { get; set; }
        public string? CPF { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
