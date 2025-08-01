using CeramicaCanelas.Domain.Entities.Financial;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Customers.Commands.UpdateCustomerCommand
{
    public class UpdateCustomerCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Document { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public void ApplyToEntity(Customer customer)
        {
            customer.Name = Name;
            customer.Document = Document;
            customer.Email = Email;
            customer.PhoneNumber = PhoneNumber;
            customer.ModifiedOn = DateTime.UtcNow;
        }
    }
}
