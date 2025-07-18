using FluentValidation;

namespace CeramicaCanelas.Application.Features.Suppliers.Commands.CreatedSuppliersCommand
{
    public class CreatedSuppliersCommandValidator : AbstractValidator<CreatedSuppliersCommand>
    {
        public CreatedSuppliersCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome do fornecedor é obrigatório.")
                .MaximumLength(100).WithMessage("O nome não pode ter mais de 100 caracteres.");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("O e-mail informado não é válido.");

            RuleFor(x => x.Cnpj)
                .MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.Cnpj));

            RuleFor(x => x.Phone)
                .MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.Phone));
        }
    }
}
