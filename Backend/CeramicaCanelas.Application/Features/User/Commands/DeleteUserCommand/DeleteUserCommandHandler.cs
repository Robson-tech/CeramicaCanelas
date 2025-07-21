using CeramicaCanelas.Application.Contracts.Infrastructure;
using CeramicaCanelas.Domain.Exception;
using MediatR;


namespace CeramicaCanelas.Application.Features.User.Commands.DeleteUserCommand
{
    public class DeleteUserCommandHandler(IIdentityAbstractor identityAbstractor) : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly IIdentityAbstractor _identityAbstractor = identityAbstractor;

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityAbstractor.FindUserByIdAsync(request.Id);

            if (user == null)
                throw new BadRequestException("Usuário não encontrado");

            var result = await _identityAbstractor.DeleteUser(user);

            if (!result.Succeeded)
                throw new BadRequestException(result);

            return Unit.Value;
        }
    }

}
