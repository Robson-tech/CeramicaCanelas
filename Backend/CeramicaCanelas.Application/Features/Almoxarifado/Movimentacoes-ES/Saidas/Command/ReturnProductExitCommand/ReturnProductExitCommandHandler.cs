using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;

namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Command.ReturnProductExitCommand
{
    public class ReturnProductExitCommandHandler : IRequestHandler<ReturnProductExitCommand, Unit>
    {
        private readonly IMovExitProductsRepository _exitRepository;
        private readonly IProductRepository _productRepository;

        public ReturnProductExitCommandHandler(IMovExitProductsRepository exitRepository, IProductRepository productRepository)
        {
            _exitRepository = exitRepository;
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(ReturnProductExitCommand request, CancellationToken cancellationToken)
        {
            var exit = await _exitRepository.GetByIdAsync(request.Id);
            if (exit == null)
                throw new BadRequestException("Saída não encontrada.");

            await ValidateReturnProductExit(request, cancellationToken);

            if (!exit.IsReturnable)
                throw new BadRequestException("Este produto não é retornável.");

            if (exit.ReturnedQuantity >= exit.Quantity)
                throw new BadRequestException("Todos os produtos já foram devolvidos.");

            if (request.QuantityReturned <= 0 || request.QuantityReturned + exit.ReturnedQuantity > exit.Quantity)
                throw new BadRequestException("Quantidade devolvida inválida.");

            var product = exit.ProductId != null
                ? await _productRepository.GetProductByIdAsync(exit.ProductId.Value)
                : null;

            // Atualiza o número de itens devolvidos
            exit.ReturnedQuantity += request.QuantityReturned;

            if (exit.ReturnedQuantity >= exit.Quantity)
            {
                exit.IsReturned = true;
                exit.ReturnDate = DateTime.UtcNow;
            }

            await _exitRepository.Update(exit);

            // Se o produto ainda existe, atualiza o estoque
            if (product != null)
            {
                product.StockCurrent += request.QuantityReturned;
                product.ModifiedOn = DateTime.UtcNow;
                await _productRepository.Update(product);
            }

            return Unit.Value;
        }


        public async Task ValidateReturnProductExit(ReturnProductExitCommand command, CancellationToken cancellationToken)
        {
            var validator = new ReturnProductExitCommandValidator();

            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult);
            }
        }
    }
}
