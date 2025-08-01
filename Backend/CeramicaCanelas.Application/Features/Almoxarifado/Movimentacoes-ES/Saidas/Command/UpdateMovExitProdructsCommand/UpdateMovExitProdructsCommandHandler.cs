using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Entities.Almoxarifado;
using CeramicaCanelas.Domain.Exception;
using MediatR;


namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Command.UpdateMovExitProdructsCommand
{
    public class UpdateMovExitProdructsCommandHandler : IRequestHandler<UpdateMovExitProdructsCommand, Unit>
    {
        private readonly ILogged _logged;
        private readonly IMovExitProductsRepository _movExitProductRepository;
        private readonly IProductRepository _productRepository;
        private readonly IEmployeesRepository _employeesRepository;

        public UpdateMovExitProdructsCommandHandler(ILogged logged, IMovExitProductsRepository movExitProductRepository, IProductRepository productRepository, IEmployeesRepository employeesRepository)
        {
            _logged = logged;
            _movExitProductRepository = movExitProductRepository;
            _productRepository = productRepository;
            _employeesRepository = employeesRepository;
        }

        public async Task<Unit> Handle(UpdateMovExitProdructsCommand command, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            var productExit = await ValidateUpdateProductExit(command, cancellationToken);

            // Reverte estoque anterior se o produto existir
            if (productExit.ProductId != null)
            {
                var previousProduct = await _productRepository.GetProductByIdAsync(productExit.ProductId.Value);
                if (previousProduct != null)
                {
                    previousProduct.StockCurrent += productExit.Quantity;
                    previousProduct.ModifiedOn = DateTime.UtcNow;
                    await _productRepository.Update(previousProduct);
                }
            }

            // Pega o produto vinculado atual para verificar estoque
            var product = productExit.ProductId != null ? await _productRepository.GetProductByIdAsync(productExit.ProductId.Value) : null;

            if (product == null)
            {
                // Produto não existe mais, só atualiza movimentação sem mexer em estoque
                productExit.ProductId = null; // opcional, pode manter também
            }
            else
            {
                // Verifica estoque para nova quantidade
                if (product.StockCurrent < command.Quantity)
                    throw new BadRequestException("Quantidade em estoque insuficiente.");

                // Atualiza estoque com a nova quantidade de saída
                product.StockCurrent -= command.Quantity;
                product.ModifiedOn = DateTime.UtcNow;
                await _productRepository.Update(product);
            }

            // Atualiza apenas os campos permitidos
            productExit.Quantity = command.Quantity;
            productExit.IsReturnable = command.IsReturnable;
            productExit.Observation = command.Observation;

            // Atualiza os metadados
            productExit.ModifiedOn = DateTime.UtcNow;
            productExit.NameOperator = user.Name;
            productExit.UserId = user.Id;

            await _movExitProductRepository.Update(productExit);

            return Unit.Value;
        }



        public async Task<ProductExit> ValidateUpdateProductExit(UpdateMovExitProdructsCommand command, CancellationToken cancellationToken)
        {
            var validator = new UpdateMovExitProdructsCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult);
            }

            var productExit = await _movExitProductRepository.GetByIdAsync(command.Id);

            if (productExit == null)
            {
                throw new BadRequestException("Movimentação de saída não encontrada.");
            }

            return productExit;
        }

    }
}
