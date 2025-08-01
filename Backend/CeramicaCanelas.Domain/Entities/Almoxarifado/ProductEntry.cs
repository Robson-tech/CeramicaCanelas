using CeramicaCanelas.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Domain.Entities.Almoxarifado
{
    public class ProductEntry : BaseEntity
    {
        public Guid? ProductId { get; set; }
        public Products? Product { get; set; } = null!;
        public string NameProduct { get; set; } = string.Empty;
        public string NameCategory { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public int Quantity { get; set; }
        public float UnitPrice { get; set; }
        public string? UserId { get; set; } = string.Empty;
        public User? User { get; set; } = null!;
        public string NameOperator { get; set; } = string.Empty;
        public Guid? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        public string NameSupplier { get; set; } = string.Empty;

    }

}
