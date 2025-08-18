using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using CeramicaCanelas.Domain.Exception;
using MediatR;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Launches.Commands.CreatedLaunchCommand
{
    public class CreatedLaunchCommandHandler : IRequestHandler<CreatedLaunchCommand, Unit>
    {
        private readonly ILogged _logged;
        private readonly ILaunchRepository _launchRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ILaunchCategoryRepository _launchCategoryRepository;
        public CreatedLaunchCommandHandler(ILogged logged, ILaunchRepository launchRepository, ICustomerRepository customerRepository, ILaunchCategoryRepository launchCategoryRepository)
        {
            _logged = logged;
            _launchRepository = launchRepository;
            _customerRepository = customerRepository;
            _launchCategoryRepository = launchCategoryRepository;

        }
        public async Task<Unit> Handle(CreatedLaunchCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null) throw new UnauthorizedAccessException("Usuário não autenticado.");

            // Normalização defensiva para binder que envia Guid.Empty
            if (request.CustomerId is Guid cid && cid == Guid.Empty) request.CustomerId = null;
            if (request.CategoryId is Guid cat && cat == Guid.Empty) request.CategoryId = null;

            // 👉 SANEAMENTO POR TIPO (mata qualquer “vazamento” do front)
            if (request.Type == LaunchType.Income)
            { // Entrada
                request.CategoryId = null;
            }
            else if (request.Type == LaunchType.Expense)
            { // Saída
                request.CustomerId = null;
            }

            await ValidateLaucnh(request, cancellationToken);

            var launch = request.AssignToLaunch();
            if (launch == null) throw new BadRequestException("Erro ao criar o lançamento financeiro.");

            // 👉 FORÇA os FKs e limpa navegação (EF não herda nada antigo)
            launch.CustomerId = request.CustomerId;
            launch.Customer = null;
            launch.CategoryId = request.CategoryId;
            launch.CategoryId = null;

            launch.OperatorName = user.UserName!;
            await _launchRepository.CreateAsync(launch, cancellationToken);
            return Unit.Value;
        }


        private async Task ValidateLaucnh(CreatedLaunchCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreatedLaunchCommandValidator();

            var validatorResponse = await validator.ValidateAsync(request, cancellationToken);

            if (!validatorResponse.IsValid)
            {
                throw new BadRequestException(validatorResponse);
            }

            if (request.CategoryId != null)
            {
                var category = await _launchCategoryRepository.GetByIdAsync(request.CategoryId.Value);
                if (category == null)
                {
                    throw new BadRequestException("Categoria de lançamento não encontrada.");
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
