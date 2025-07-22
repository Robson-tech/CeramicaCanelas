using FluentValidation;

namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Command.UpdateMovExitProdructsCommand
{
    public class UpdateMovExitProdructsCommandValidator : AbstractValidator<UpdateMovExitProdructsCommand>
    {
        public UpdateMovExitProdructsCommandValidator()
        {

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");

            RuleFor(x => x.Observation)
                .MaximumLength(300).WithMessage("A observação deve ter no máximo 300 caracteres.");

        }
    }
}
