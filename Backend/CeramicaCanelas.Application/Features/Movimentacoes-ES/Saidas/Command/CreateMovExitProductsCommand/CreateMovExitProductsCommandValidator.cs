using FluentValidation;

namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Command.CreateMovExitProductsCommand
{
    public class CreateMovExitProductsCommandValidator : AbstractValidator<CreateMovExitProductsCommand>
    {
        public CreateMovExitProductsCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("O ID do produto é obrigatório.");

            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("O ID do funcionário é obrigatório.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");

            RuleFor(x => x.Observation)
                .MaximumLength(300).WithMessage("A observação deve ter no máximo 300 caracteres.");
        }
    }
}
