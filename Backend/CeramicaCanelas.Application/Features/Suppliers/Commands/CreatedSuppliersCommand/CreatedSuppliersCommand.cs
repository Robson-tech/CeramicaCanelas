using CeramicaCanelas.Domain.Entities;
using MediatR;

namespace CeramicaCanelas.Application.Features.Suppliers.Commands.CreatedSuppliersCommand
{
    public class CreatedSuppliersCommand : IRequest<Unit>
    {
        public string Name { get; set; } = string.Empty;
        public string? Cnpj { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        public Supplier AssignToSupplier()
        {
            return new Supplier
            {
                Name = Name,
                Cnpj = Cnpj,
                Phone = Phone,
                Email = Email,
                Address = Address,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow
            };
        }
    }
}
