using CeramicaCanelas.Domain.Entities.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Customers.Queries.GetPagedCustomerQueries
{
    public class CustomerResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Document { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public CustomerResult(Customer customer)
        {
            Id = customer.Id;
            Name = customer.Name;
            Document = customer.Document;
            Email = customer.Email;
            PhoneNumber = customer.PhoneNumber;
        }
    }
}
