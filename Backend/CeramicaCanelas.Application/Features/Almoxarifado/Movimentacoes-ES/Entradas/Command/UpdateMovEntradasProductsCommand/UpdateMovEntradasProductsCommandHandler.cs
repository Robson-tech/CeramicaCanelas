using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Entities.Almoxarifado;
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
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            var movimentacaoES = await ValidateUpdateMovimentacaoES(command, cancellationToken);

            // Desfaz o estoque antigo, se o produto ainda existir
            if (movimentacaoES.ProductId != null)
            {
                var oldProduct = await _productRepository.GetProductByIdAsync(movimentacaoES.ProductId.Value);
                if (oldProduct != null)
                {
                    oldProduct.StockCurrent -= movimentacaoES.Quantity;
                    oldProduct.ValueTotal -= movimentacaoES.UnitPrice * movimentacaoES.Quantity;
                    oldProduct.ModifiedOn = DateTime.UtcNow;
                    await _productRepository.Update(oldProduct);
                }
            }

            // Busca o produto vinculado à movimentação
            var product = movimentacaoES.ProductId != null
                ? await _productRepository.GetProductByIdAsync(movimentacaoES.ProductId.Value)
                : null;

            // Busca o fornecedor
            var supplier = await _supplierRepository.GetByIdAsync(command.SupplierId);
            if (supplier == null)
                throw new BadRequestException("Fornecedor não encontrado.");

            if (product != null)
            {
                movimentacaoES.ProductId = product.Id;
                movimentacaoES.NameProduct = product.Name;
                movimentacaoES.NameCategory = product.Category!.Name;

                // Atualiza estoque com os novos valores
                product.StockCurrent += command.Quantity;
                product.ValueTotal += command.UnitPrice * command.Quantity;
                product.ModifiedOn = DateTime.UtcNow;
                await _productRepository.Update(product);
            }
            // 👉 Se o produto não existir mais:
            // - mantém NameProduct e NameCategory já existentes
            // - apenas zera o ProductId (se quiser)
            else
            {
                movimentacaoES.ProductId = null;
            }

            // Atualiza a movimentação com os novos dados
            movimentacaoES.SupplierId = command.SupplierId;
            movimentacaoES.NameSupplier = supplier.Name;
            movimentacaoES.Quantity = command.Quantity;
            movimentacaoES.UnitPrice = command.UnitPrice;
            movimentacaoES.ModifiedOn = DateTime.UtcNow;
            movimentacaoES.UserId = user.Id;
            movimentacaoES.NameOperator = user.Name;

            await _movimentacaoESRepository.Update(movimentacaoES);

            return Unit.Value;
        }


        private async Task<ProductEntry> ValidateUpdateMovimentacaoES(UpdateMovEntradasProductsCommand command, CancellationToken cancellationToken)
        {
            var validator = new UpdateMovEntradasProductsCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                throw new BadRequestException(validationResult);

            var movimentacaoES = await _movimentacaoESRepository.GetByIdAsync(command.Id);
            if (movimentacaoES == null)
                throw new BadRequestException("Movimentação de entrada não encontrada.");

            return movimentacaoES;
        }


    }
}
