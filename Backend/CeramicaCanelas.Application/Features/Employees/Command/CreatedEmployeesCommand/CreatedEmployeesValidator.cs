using FluentValidation;

namespace CeramicaCanelas.Application.Features.Employees.Command.CreatedEmployeesCommand
{
    public class CreatedEmployeesValidator : AbstractValidator<CreatedEmployeesCommand>
    {
        public CreatedEmployeesValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(100).WithMessage("O nome não pode exceder 100 caracteres.");
            RuleFor(x => x.CPF)
                .NotEmpty().WithMessage("O CPF é obrigatório.")
                .Matches(@"^\d{11}$").WithMessage("O CPF deve conter exatamente 11 dígitos.");
            RuleFor(x => x.Positiions)
                .IsInEnum().WithMessage("O cargo informada é inválido.");
        }
    }
}
