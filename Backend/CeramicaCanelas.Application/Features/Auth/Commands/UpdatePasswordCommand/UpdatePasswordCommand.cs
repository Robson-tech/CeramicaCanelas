using MediatR;


namespace CeramicaCanelas.Application.Features.Auth.Commands.UpdatePasswordCommand
{
    public class UpdatePasswordCommand : IRequest<Unit>
    {
        public string Email { get; set; } = string.Empty;
    }
}

