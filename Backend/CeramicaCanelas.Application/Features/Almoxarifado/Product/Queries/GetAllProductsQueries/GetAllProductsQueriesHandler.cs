using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Application.Features.Employees.Queries.GetAllEmployeesQueries;
using CeramicaCanelas.Domain.Entities;
using CeramicaCanelas.Domain.Exception;
using MediatR;

namespace CeramicaCanelas.Application.Features.Almoxarifado.Product.Queries.GetAllProductsQueries
{

    public class GetAllProductsQueries : IRequest<List<GetAllProductsQueriesResult>> { }
    public class GetAllProductsQueriesHandler : IRequestHandler<GetAllProductsQueries, List<GetAllProductsQueriesResult>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogged _logged;

        public GetAllProductsQueriesHandler(IProductRepository productRepository, ILogged logged)
        {
            _productRepository = productRepository;
            _logged = logged;
        }

        public async Task<List<GetAllProductsQueriesResult>> Handle(GetAllProductsQueries request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            var products = await _productRepository.GetAllProductsAsync();
            if (!products.Any())
                throw new BadRequestException("Não há produtos cadastrados.");

            return products.Select(product => new GetAllProductsQueriesResult(product)).ToList();
        }
    }

}
