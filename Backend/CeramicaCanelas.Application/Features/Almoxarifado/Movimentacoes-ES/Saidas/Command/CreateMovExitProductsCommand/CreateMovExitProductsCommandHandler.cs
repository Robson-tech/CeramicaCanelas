
using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;

namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Command.CreateMovExitProductsCommand
{
    public class CreateMovExitProductsCommandHandler : IRequestHandler<CreateMovExitProductsCommand, Unit>
    {
        private readonly IMovExitProductsRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ILogged _logged;

        public CreateMovExitProductsCommandHandler(ILogged logged, IMovExitProductsRepository repository, IProductRepository productRepository, IEmployeesRepository employeesRepository)
        {
            _logged = logged;
            _repository = repository;
            _productRepository = productRepository;
            _employeesRepository = employeesRepository;
        }

        public async Task<Unit> Handle(CreateMovExitProductsCommand command, CancellationToken cancellationToken)
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


            if (product.StockCurrent < command.Quantity)
            {
                throw new BadRequestException("Quantidade em estoque insuficiente.");
            }

            var employee =  await _employeesRepository.GetByIdAsync(command.EmployeeId);
            if (employee == null)
            {
                throw new BadRequestException("Funcionário não encontrado");
            }

            await ValidateProductExit(command, cancellationToken);

            var moveExit = command.AssignToProductsExit();
            moveExit.UserId = user.Id;
            moveExit.NameProduct = product.Name;
            moveExit.EmployeeName = employee.Name;
            moveExit.NameOperator = user.Name;

            await _repository.CreateAsync(moveExit, cancellationToken);

            //Atualizar dados do produto no estoque
            //ATUALIZA O ESTOQUE DO PRODUTO
            product.StockCurrent -= command.Quantity;
            product.ModifiedOn = DateTime.UtcNow;
            await _productRepository.Update(product);

            return Unit.Value;
        }

        public async Task ValidateProductExit(CreateMovExitProductsCommand command, CancellationToken cancellationToken)
        {
            var validator = new CreateMovExitProductsCommandValidator();
            var resultValidator = await validator.ValidateAsync(command, cancellationToken);

            if (!resultValidator.IsValid)
            {
                throw new BadRequestException(resultValidator);
            }
        }



    }
}
