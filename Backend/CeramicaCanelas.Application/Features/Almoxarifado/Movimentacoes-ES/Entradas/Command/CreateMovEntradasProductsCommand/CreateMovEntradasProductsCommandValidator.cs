using FluentValidation;

namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Entradas.Command.CreateMovEntradasProductsCommand
{
    public class CreateMovEntradasProductsCommandValidator : AbstractValidator<CreateMovEntradasProductCommand>
    {
        public CreateMovEntradasProductsCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("O ID do produto é obrigatório.");
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");
            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("O preço unitário deve ser maior que zero.");

        }
    }
}

