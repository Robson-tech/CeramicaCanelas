using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Launches.Commands.UpdateLaunchCommand
{
    public class UpdateLaunchCommandValidator : AbstractValidator<UpdateLaunchCommand>
    {
        public UpdateLaunchCommandValidator()
        {
            RuleFor(l => l.Id)
                .NotEmpty().WithMessage("O ID do lançamento é obrigatório.");

            RuleFor(l => l.Description)
                .NotEmpty().WithMessage("A descrição é obrigatória.")
                .MaximumLength(200).WithMessage("A descrição não pode exceder 200 caracteres.");

            RuleFor(l => l.Amount)
                .GreaterThan(0).WithMessage("O valor do lançamento deve ser maior que zero.");

            RuleFor(l => l.LaunchDate)
                .NotEmpty().WithMessage("A data do lançamento é obrigatória.");

            RuleFor(l => l.Type)
                .IsInEnum().WithMessage("O tipo de lançamento é inválido.");

            RuleFor(l => l.PaymentMethod)
                .IsInEnum().WithMessage("A forma de pagamento é inválida.");

            RuleFor(l => l.Status)
                .IsInEnum().WithMessage("O status do pagamento é inválido.");
        }
    }
}
