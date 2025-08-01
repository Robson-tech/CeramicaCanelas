using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Launches.Commands.DeleteLaunchCommand
{
    public class DeleteLaunchCommandValidator : AbstractValidator<DeleteLaunchCommand>
    {
        public DeleteLaunchCommandValidator()
        {
            RuleFor(l => l.Id)
                .NotEmpty().WithMessage("O ID do lançamento é obrigatório para a exclusão.");
        }
    }
}
