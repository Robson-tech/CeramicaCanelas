using CeramicaCanelas.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Domain.Entities
{
    public class ProductEntry : BaseEntity
    {
        public Guid? ProductId { get; set; }
        public Products? Product { get; set; } = null!;

        public DateTime EntryDate { get; set; }
        public int Quantity { get; set; }
        public float UnitPrice { get; set; }
        public string? UserId { get; set; } = string.Empty;
        public User? User { get; set; } = null!;
        public Guid? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

    }

}
