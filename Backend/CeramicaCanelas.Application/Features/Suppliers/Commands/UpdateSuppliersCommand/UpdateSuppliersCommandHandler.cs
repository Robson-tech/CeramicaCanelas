using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Domain.Exception;
using CeramicaCanelas.Persistence.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Suppliers.Commands.UpdateSuppliersCommand
{
    public class UpdateSuppliersCommandHandler : IRequestHandler<UpdateSuppliersCommand, Unit>
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ILogged _logged;

        public UpdateSuppliersCommandHandler(ISupplierRepository supplierRepository, ILogged logged)
        {
            _supplierRepository = supplierRepository;
            _logged = logged;
        }

        public async Task<Unit> Handle(UpdateSuppliersCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user is null)
                throw new BadRequestException("Usuário não encontrado.");

            await ValidateUpdate(request, cancellationToken);

            var supplier = await _supplierRepository.GetByIdAsync(request.Id);
            if (supplier == null)
                throw new BadRequestException("Fornecedor não encontrado.");

            request.AssignToEntity(supplier);
            await _supplierRepository.Update(supplier);

            return Unit.Value;
        }

        private async Task ValidateUpdate(UpdateSuppliersCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateSuppliersCommandValidator();
            var result = await validator.ValidateAsync(request, cancellationToken);

            if (!result.IsValid)
                throw new BadRequestException(result);
        }
    }
}
