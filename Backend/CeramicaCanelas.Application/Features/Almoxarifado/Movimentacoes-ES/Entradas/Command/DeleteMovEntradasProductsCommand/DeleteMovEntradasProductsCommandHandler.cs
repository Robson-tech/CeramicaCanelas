using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Entradas.Command.DeleteMovEntradasProductsCommand
{
    public class DeleteMovEntradasProductsCommandHandler : IRequestHandler<DeleteMovEntradasProductsCommand, Unit>
    {
        private readonly IMovEntryProductsRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly ILogged _logged;

        public DeleteMovEntradasProductsCommandHandler(IMovEntryProductsRepository repository, ILogged logged, IProductRepository productRepository)
        {
            _repository = repository;
            _logged = logged;
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(DeleteMovEntradasProductsCommand command, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();

            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            var movimentacaoES = await _repository.GetByIdAsync(command.Id);

            if (movimentacaoES == null)
                throw new BadRequestException("Movimentação de entrada não encontrada.");

            // Atualiza o estoque do produto

            if (movimentacaoES.ProductId == null)
            {
                throw new BadRequestException("Produto associado à movimentação está nulo.");
            }

            var product = await _productRepository.GetProductByIdAsync(movimentacaoES.ProductId);
            if (product == null)
            {
                throw new BadRequestException("Produto não encontrado.");
            }
            //Desatalizando a movimentação anterior
            product.StockCurrent -= movimentacaoES.Quantity;
            product.ValueTotal -= movimentacaoES.UnitPrice * movimentacaoES.Quantity;
            product.ModifiedOn = DateTime.UtcNow;
            await _productRepository.Update(product);

            // Exclui a movimentação de entrada
            await _repository.Delete(movimentacaoES);

            return Unit.Value;
        }

    }

}

