

using CeramicaCanelas.Domain.Entities;

namespace CeramicaCanelas.Application.Features.Suppliers.Queries.Pages
{
    public class SupplierResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }

        public string? Cnpj { get; set; } = string.Empty;
        public string? Phone { get; set; }

        public SupplierResult(Supplier supplier)
        {
            Id = supplier.Id;
            Name = supplier.Name;
            Email = supplier.Email;
            Cnpj = supplier.Cnpj;
            Phone = supplier.Phone;
        }
    }
}
