using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Entradas.Command.UpdateMovEntradasProductsCommand
{
    public class UpdateMovEntradasProductsCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public int Quantity { get; set; }
        public float UnitPrice { get; set; }

    }
}
