using CeramicaCanelas.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Suppliers.Commands.UpdateSuppliersCommand
{
    public class UpdateSuppliersCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Cnpj { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        public void AssignToEntity(Supplier supplier)
        {
            supplier.Name = Name;
            supplier.Cnpj = Cnpj;
            supplier.Phone = Phone;
            supplier.Email = Email;
            supplier.Address = Address;
            supplier.ModifiedOn = DateTime.UtcNow;
        }
    }
}
