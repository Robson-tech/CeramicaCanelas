using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;


namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Customers.Commands.DeleteCustomerCommand
{
    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Unit>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogged _logged;

        public DeleteCustomerCommandHandler(ICustomerRepository customerRepository, ILogged logged)
        {
            _customerRepository = customerRepository;
            _logged = logged;
        }

        public async Task<Unit> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            var customer = await _customerRepository.GetByIdAsync(request.Id);

            if (customer == null || customer.IsDeleted)
                throw new BadRequestException("Cliente não encontrado ou já removido.");

            customer.IsDeleted = true;
            customer.ModifiedOn = DateTime.UtcNow;

            await _customerRepository.Update(customer);

            return Unit.Value;
        }
    }
}
