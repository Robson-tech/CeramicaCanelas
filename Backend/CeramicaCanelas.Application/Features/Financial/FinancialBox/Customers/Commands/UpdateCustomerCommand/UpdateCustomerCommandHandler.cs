using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Customers.Commands.UpdateCustomerCommand
{
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Unit>
    {
        private readonly ILogged _logged;
        private readonly ICustomerRepository _customerRepository;

        public UpdateCustomerCommandHandler(ICustomerRepository customerRepository, ILogged logged)
        {
            _customerRepository = customerRepository;
            _logged = logged;
        }

        public async Task<Unit> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            var customer = await _customerRepository.GetByIdAsync(request.Id);

            if (customer == null || customer.IsDeleted)
                throw new BadRequestException("Cliente não encontrado.");

            await ValidateCustomer(request, cancellationToken);

            request.ApplyToEntity(customer);

            await _customerRepository.Update(customer);

            return Unit.Value;
        }

        public async Task ValidateCustomer(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateCustomerCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult);
            }
        }
    }
}
