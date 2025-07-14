using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Command.ReturnProductExitCommand
{
    public class ReturnProductExitCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public int QuantityReturned { get; set; }


    }
}
