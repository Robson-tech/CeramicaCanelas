using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using CeramicaCanelas.Persistence.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Entradas.Command.UpdateMovEntradasProductsCommand
{
    public class UpdateMovEntradasProductsCommandHandler : IRequestHandler<UpdateMovEntradasProductsCommand, Unit>
    {
        private readonly IMovEntryProductsRepository _movimentacaoESRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogged _logged;

        public UpdateMovEntradasProductsCommandHandler(IMovEntryProductsRepository movimentacaoEntradasProductsRepository,
            IProductRepository productRepository,
            ILogged logged,
            ISupplierRepository supplierRepository)

        {
            _movimentacaoESRepository = movimentacaoEntradasProductsRepository;
            _productRepository = productRepository;
            _logged = logged;
            _supplierRepository = supplierRepository;
        }

        public async Task<Unit> Handle(UpdateMovEntradasProductsCommand command, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();

            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            var movimentacaoES = await ValidateUpdateMovimentacaoES(command, cancellationToken);

            // Atualiza o estoque do produto
            var product = await _productRepository.GetProductByIdAsync(movimentacaoES.ProductId);
            if (product == null)
            {
                throw new BadRequestException("Produto não encontrado.");
            }

            var supplier = await _supplierRepository.GetByIdAsync(command.SupplierId);
            if (supplier == null)
            {
                throw new BadRequestException("Fornecedor não encontrado.");
            }

            //Desatalizando a movimentação anterior
            product.StockCurrent -= movimentacaoES.Quantity;
            product.ValueTotal -= movimentacaoES.UnitPrice * movimentacaoES.Quantity;
            await _productRepository.Update(product);


            //Atualizando com os novos valores
            movimentacaoES.ProductId = movimentacaoES.ProductId;
            movimentacaoES.NameCategory = product.Category!.Name;
            movimentacaoES.NameProduct = product.Name;
            movimentacaoES.SupplierId = command.SupplierId;
            movimentacaoES.NameSupplier = supplier.Name;
            movimentacaoES.Quantity = command.Quantity;
            movimentacaoES.UnitPrice = command.UnitPrice;
            movimentacaoES.ModifiedOn = DateTime.UtcNow;
            movimentacaoES.UserId = user.Id;
            movimentacaoES.NameOperator = user.Name;
            await _movimentacaoESRepository.Update(movimentacaoES);

            // Atualiza o estoque do produto com os novos valores
            product.StockCurrent += command.Quantity;
            product.ValueTotal += command.UnitPrice * command.Quantity;
            product.ModifiedOn = DateTime.UtcNow;
            await _productRepository.Update(product);

            return Unit.Value;
        }

        private async Task<Domain.Entities.ProductEntry> ValidateUpdateMovimentacaoES(UpdateMovEntradasProductsCommand command, CancellationToken cancellationToken)
        {
            var movimentacaoES = await _movimentacaoESRepository.GetByIdAsync(command.Id);
            if (movimentacaoES == null)
            {
                throw new BadRequestException("Movimentação de entrada não encontrada.");
            }

            var product = await _productRepository.GetProductByIdAsync(movimentacaoES.ProductId);
            if (product == null)
            {
                throw new BadRequestException("Produto não encontrado.");
            }

            var validator = new UpdateMovEntradasProductsCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult);
            }

            return movimentacaoES;
        }

    }
}
