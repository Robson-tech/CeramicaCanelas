using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;


namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Customers.Commands.CreateCustomerCommand
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Unit>
    {
        private readonly ILogged _logged;
        private readonly ICustomerRepository _customerRepository;

        public CreateCustomerCommandHandler(ICustomerRepository customerRepository, ILogged logged)
        {
            _customerRepository = customerRepository;
            _logged = logged;
        }

        public async Task<Unit> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            // Agora a variável '_logged' não será mais nula
            var user = await _logged.UserLogged();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            await ValidateCustomer(request, cancellationToken);

            var customer = request.AssignToEntity();

            await _customerRepository.CreateAsync(customer, cancellationToken);

            return Unit.Value;
        }

        public async Task ValidateCustomer(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateCustomerCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new BadRequestException(validationResult);

        }
    }
}
