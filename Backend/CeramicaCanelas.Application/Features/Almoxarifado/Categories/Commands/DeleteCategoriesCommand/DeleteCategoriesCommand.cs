using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.Categories.Commands.DeleteCategoriesCommand
{
    public class DeleteCategoriesCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
