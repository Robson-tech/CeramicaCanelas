using FluentValidation;

namespace CeramicaCanelas.Application.Features.User.Commands.CreateUserCommand
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(cpau => cpau.Email)
                .NotNull()
                .NotEmpty()
                .WithMessage("O campo e-mail precisa ser preenchido");

            RuleFor(cpau => cpau.Email)
                .EmailAddress()
                .WithMessage("O campo e-mail precisa ser um e-mail válido");

            RuleFor(cpau => cpau.Password)
                .NotEmpty()
                .MinimumLength(6).WithMessage("O campo senha precisa ter, pelo menos, 6 caracteres")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$")
                .WithMessage("A senha deve conter pelo menos uma letra maiúscula, uma letra minúscula e um número.")
                .Matches(@"[!@#$%^&*(),.?""{}|<>]")
                .WithMessage("A senha deve conter pelo menos um caractere especial.");

            RuleFor(cpau => cpau.PasswordConfirmation)
                .Equal(cpau => cpau.Password)
                .WithMessage("O campo de confirmação de senha deve ser igual ao campo senha");
        }
    }
}
