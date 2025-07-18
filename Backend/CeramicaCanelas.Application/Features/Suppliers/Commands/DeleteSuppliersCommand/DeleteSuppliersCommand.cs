using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Suppliers.Commands.DeleteSuppliersCommand
{
    public class DeleteSuppliersCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
