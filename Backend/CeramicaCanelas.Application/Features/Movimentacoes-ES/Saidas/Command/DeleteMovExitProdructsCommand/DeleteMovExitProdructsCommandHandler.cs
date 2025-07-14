using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Command.DeleteMovExitProdructsCommand
{
    public class DeleteMovExitProdructsCommandHandler : IRequestHandler<DeleteMovExitProdructsCommand, Unit>
    {
        private readonly IMovExitProductsRepository _movExitProductRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogged _logged;
        public DeleteMovExitProdructsCommandHandler(ILogged logged, IProductRepository productRepository, IMovExitProductsRepository movExitProductsRepository)
        {
            _logged = logged;
            _productRepository = productRepository;
            _movExitProductRepository = movExitProductsRepository;
        }

        public async Task<Unit> Handle(DeleteMovExitProdructsCommand command, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }
            var productExit = await _movExitProductRepository.GetByIdAsync(command.Id);
            if (productExit == null)
            {
                throw new BadRequestException("Movimentação de saída não encontrada.");
            }
            // Atualiza o estoque do produto
            var product = await _productRepository.GetProductByIdAsync(productExit.ProductId);
            if (product == null)
            {
                throw new BadRequestException("Produto não encontrado.");
            }
            product.StockCurrent += productExit.Quantity;
            product.ModifiedOn = DateTime.UtcNow;
            await _productRepository.Update(product);
            await _movExitProductRepository.Delete(productExit);
            return Unit.Value;
        }
    }
}
