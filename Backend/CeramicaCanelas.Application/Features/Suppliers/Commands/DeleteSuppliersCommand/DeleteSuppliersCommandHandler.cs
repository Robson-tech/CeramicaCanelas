using CeramicaCanelas.Domain.Exception;
using CeramicaCanelas.Persistence.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Suppliers.Commands.DeleteSuppliersCommand
{
    public class DeleteSuppliersCommandHandler : IRequestHandler<DeleteSuppliersCommand, Unit>
    {
        private readonly ISupplierRepository _supplierRepository;

        public DeleteSuppliersCommandHandler(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<Unit> Handle(DeleteSuppliersCommand request, CancellationToken cancellationToken)
        {
            var supplier = await _supplierRepository.GetByIdAsync(request.Id);

            if (supplier is null)
                throw new BadRequestException("Fornecedor não encontrado.");

            await _supplierRepository.Delete(supplier);

            return Unit.Value;
        }
    }
}
