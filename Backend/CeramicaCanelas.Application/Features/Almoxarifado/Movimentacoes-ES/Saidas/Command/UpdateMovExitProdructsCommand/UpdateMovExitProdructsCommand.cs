using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Command.UpdateMovExitProdructsCommand
{
    public class UpdateMovExitProdructsCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public bool IsReturnable { get; set; }
        public string? Observation { get; set; }
    }
}
