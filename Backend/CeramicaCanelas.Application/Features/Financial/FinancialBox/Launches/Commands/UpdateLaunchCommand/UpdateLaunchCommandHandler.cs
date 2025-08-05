using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Entities.Financial;
using CeramicaCanelas.Domain.Exception;
using MediatR;


namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Launches.Commands.UpdateLaunchCommand
{


    public class UpdateLaunchCommandHandler : IRequestHandler<UpdateLaunchCommand, Unit>
    {
        private readonly ILogged _logged;
        private readonly ILaunchRepository _launchRepository;
        private readonly ILaunchCategoryRepository _launchCategoryRepository;
        private readonly ICustomerRepository _customerRepository;

        // Construtor idêntico ao de Create, apenas com o repositório necessário.
        public UpdateLaunchCommandHandler(ILogged logged, ILaunchRepository launchRepository, ILaunchCategoryRepository launchCategoryRepository, ICustomerRepository customerRepository)
        {
            _logged = logged;
            _launchRepository = launchRepository;
            _launchCategoryRepository = launchCategoryRepository;
            _customerRepository = customerRepository;
        }

        public async Task<Unit> Handle(UpdateLaunchCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            // 1. Valida o comando de entrada
            await ValidateLaunch(request, cancellationToken);

            // 2. Busca o lançamento existente no banco
            var launchToUpdate = await _launchRepository.GetByIdAsync(request.Id);
            if (launchToUpdate == null)
            {
                throw new BadRequestException("Lançamento não encontrado.");
            }

            // 3. Mapeia os novos dados para a entidade existente
            request.MapToLaunch(launchToUpdate);

            launchToUpdate.OperatorName = user.UserName!;

            // 4. Chama o método de atualização do repositório
            await _launchRepository.Update(launchToUpdate);

            return Unit.Value;
        }

        // Método privado de validação, exatamente como no seu Create Handler
        private async Task ValidateLaunch(UpdateLaunchCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateLaunchCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult);
            }

            if (request.CategoryId != null)
            {
                var category = await _launchCategoryRepository.GetByIdAsync(request.CategoryId.Value);
                if (category == null)
                {
                    throw new BadRequestException("Categoria não encontrada.");
                }
            }

            if (request.CustomerId != null)
            {
                var customer = await _customerRepository.GetByIdAsync(request.CustomerId.Value);
                if (customer == null)
                {
                    throw new BadRequestException("Cliente não encontrado.");
                }
            }
        }
    }
}
