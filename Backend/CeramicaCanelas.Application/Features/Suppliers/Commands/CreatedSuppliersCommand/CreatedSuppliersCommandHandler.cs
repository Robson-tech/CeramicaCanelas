using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using CeramicaCanelas.Persistence.Repositories;
using MediatR;

namespace CeramicaCanelas.Application.Features.Suppliers.Commands.CreatedSuppliersCommand
{
    public class CreatedSuppliersCommandHandler : IRequestHandler<CreatedSuppliersCommand, Unit>
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ILogged _logged;

        public CreatedSuppliersCommandHandler(ISupplierRepository supplierRepositor, ILogged logged)
        {
            _supplierRepository = supplierRepositor;
            _logged = logged;
        }

        public async Task<Unit> Handle(CreatedSuppliersCommand request, CancellationToken cancellationToken)
        {

            var user = await _logged.UserLogged();

            if (user is null)
            {
                throw new BadRequestException("Usuário não encontrado.");
            }

            await ValidateCreatedSupplier(request, cancellationToken);

            var newSupplier = request.AssignToSupplier();
            await _supplierRepository.CreateAsync(newSupplier);
            return Unit.Value;
        }

        public async Task ValidateCreatedSupplier(CreatedSuppliersCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreatedSuppliersCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult);
            }
        }
    }
}
