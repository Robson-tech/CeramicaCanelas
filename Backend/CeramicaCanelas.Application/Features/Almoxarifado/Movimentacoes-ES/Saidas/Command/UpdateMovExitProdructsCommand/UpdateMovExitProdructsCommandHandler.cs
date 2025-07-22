using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
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
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            var product = await _productRepository.GetProductByIdAsync(command.ProductId);

            if (product == null)
            {
                throw new BadRequestException("Produto não encontrado");
            }

            var employee = await _employeesRepository.GetByIdAsync(command.EmployeeId);
            if (employee == null)
            {
                throw new BadRequestException("Funcionário não encontrado");
            }

            var productExit = await ValidateUpdateProductExit(command, cancellationToken);

            //Desatalizando a movimentação anterior
            product.StockCurrent += productExit.Quantity;
            product.ModifiedOn = DateTime.UtcNow;
            
            await _productRepository.Update(product);

            if (product.StockCurrent < command.Quantity)
            {
                throw new BadRequestException("Quantidade em estoque insuficiente.");
            }

            //Atualizando com os novos valores
            productExit.NameProduct = product.Name;
            productExit.EmployeeName = employee.Name;
            productExit.NameOperator = user.Name;
            productExit.ProductId = command.ProductId;
            productExit.EmployeeId = command.EmployeeId;
            productExit.Quantity = command.Quantity;
            productExit.Observation = command.Observation;
            productExit.IsReturnable = command.IsReturnable;
            productExit.ExitDate = DateTime.UtcNow;
            productExit.ModifiedOn = DateTime.UtcNow;
            productExit.IsReturned = false;
            productExit.UserId = user.Id;

            await _movExitProductRepository.Update(productExit);

            // Atualiza o estoque do produto com os novos valores
            product.StockCurrent -= command.Quantity;
            product.ModifiedOn = DateTime.UtcNow;
            await _productRepository.Update(product);

            return Unit.Value;

        }

        public async Task<Domain.Entities.ProductExit> ValidateUpdateProductExit(UpdateMovExitProdructsCommand command, CancellationToken cancellationToken)
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
