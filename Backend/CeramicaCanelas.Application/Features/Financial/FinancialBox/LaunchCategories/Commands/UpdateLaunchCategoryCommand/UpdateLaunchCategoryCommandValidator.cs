using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.LaunchCategories.Commands.UpdateLaunchCategoryCommand
{
    public class UpdateLaunchCategoryCommandValidator : AbstractValidator<UpdateLaunchCategoryCommand>
    {
        public UpdateLaunchCategoryCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("O ID da categoria é obrigatório.");

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
                .MaximumLength(100).WithMessage("O nome não pode exceder 100 caracteres.");
        }
    }
}
