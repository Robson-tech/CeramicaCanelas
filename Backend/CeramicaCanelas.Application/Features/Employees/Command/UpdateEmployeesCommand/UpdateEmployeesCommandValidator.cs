using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Employees.Command.UpdateEmployeesCommand
{
    public class UpdateEmployeesCommandValidator : AbstractValidator<UpdateEmployeesCommand>
    {
        public UpdateEmployeesCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(100).WithMessage("O nome não pode exceder 100 caracteres.");
            RuleFor(x => x.Positiions)
                .IsInEnum().WithMessage("O cargo informado é inválido.");

        }
    }
}
