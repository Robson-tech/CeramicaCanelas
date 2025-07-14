using FluentValidation;

namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Command.ReturnProductExitCommand
{
    public class ReturnProductExitCommandValidator : AbstractValidator<ReturnProductExitCommand>
    {
        public ReturnProductExitCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O ID da movimentação de saída é obrigatório.");

            RuleFor(x => x.QuantityReturned)
               .GreaterThan(0).WithMessage("A quantidade devolvida deve ser maior que zero.");
        }
    }
}
