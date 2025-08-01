using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Customers.Commands.DeleteCustomerCommand
{
    public class DeleteCustomerCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
