using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;


namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Launches.Commands.DeleteLaunchCommand
{
    public class DeleteLaunchCommandHandler : IRequestHandler<DeleteLaunchCommand, Unit>
    {
        private readonly ILogged _logged;
        private readonly ILaunchRepository _launchRepository;

        public DeleteLaunchCommandHandler(ILogged logged, ILaunchRepository launchRepository)
        {
            _logged = logged;
            _launchRepository = launchRepository;
        }

        public async Task<Unit> Handle(DeleteLaunchCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            var validator = new DeleteLaunchCommandValidator();
            var validatorResponse = await validator.ValidateAsync(request, cancellationToken);
            if (!validatorResponse.IsValid)
            {
                throw new BadRequestException(validatorResponse);
            }

            var launch = await _launchRepository.GetByIdAsync(request.Id);
            if (launch == null)
            {
                throw new BadRequestException("Lançamento financeiro não encontrado.");
            }

            await _launchRepository.Delete(launch);
            return Unit.Value;
        }
    }

}
