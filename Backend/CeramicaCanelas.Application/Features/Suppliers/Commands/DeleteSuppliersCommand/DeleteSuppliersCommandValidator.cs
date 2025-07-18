using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Suppliers.Commands.DeleteSuppliersCommand
{
    public class DeleteSuppliersCommandValidator : AbstractValidator<DeleteSuppliersCommand>
    {
        public DeleteSuppliersCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O ID do fornecedor é obrigatório.");
        }
    }
}
