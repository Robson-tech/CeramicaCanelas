using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.Product.Commands.CreatedProductCommand
{
    public class CreatedProductCommandValidator : AbstractValidator<CreatedProductCommand>
    {
        public CreatedProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(100).WithMessage("O nome não pode exceder 100 caracteres.");
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("O código é obrigatório.")
                .MaximumLength(50).WithMessage("O código não pode exceder 50 caracteres.");
            RuleFor(x => x.UnitOfMeasure)
                .IsInEnum().WithMessage("A unidade de medida informada é inválida.");
            RuleFor(x => x.StockInitial)
                .GreaterThanOrEqualTo(0).WithMessage("O estoque inicial deve ser maior ou igual a zero.");
            RuleFor(x => x.StockMinium)
                .GreaterThanOrEqualTo(0).WithMessage("O estoque mínimo deve ser maior ou igual a zero.");
        }
    }
}
