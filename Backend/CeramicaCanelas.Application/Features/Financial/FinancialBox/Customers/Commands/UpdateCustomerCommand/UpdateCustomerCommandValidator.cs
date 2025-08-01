using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Customers.Commands.UpdateCustomerCommand
{
    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("O ID do cliente é obrigatório.");

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(100).WithMessage("O nome não pode exceder 100 caracteres.");

            RuleFor(c => c.Email)
                .EmailAddress().When(c => !string.IsNullOrWhiteSpace(c.Email))
                .WithMessage("O e-mail informado não é válido.");

            RuleFor(c => c.PhoneNumber)
                .MaximumLength(20).WithMessage("O telefone não pode exceder 20 caracteres.");
        }
    }
}
