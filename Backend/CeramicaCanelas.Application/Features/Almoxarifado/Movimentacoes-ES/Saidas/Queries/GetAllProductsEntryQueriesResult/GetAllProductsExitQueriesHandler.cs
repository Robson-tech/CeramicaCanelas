using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Application.Features.Movimentacoes_ES.Entradas.Queries.GetAllProductsEntryQueries;
using CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Queries.CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Queries;
using CeramicaCanelas.Domain.Exception;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Saidas.Queries
{
    public class GetAllProductsExitQueries : IRequest<List<GetAllProductsExitQueriesResult>> { }
    public class GetAllProductsExitQueriesHandler : IRequestHandler<GetAllProductsExitQueries, List<GetAllProductsExitQueriesResult>>
    {
        private readonly IMovExitProductsRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ILogged _logged;

        public GetAllProductsExitQueriesHandler(IMovExitProductsRepository repository, IProductRepository productRepository, IEmployeesRepository employeesRepository, ILogged logged)
        {
            _repository = repository;
            _productRepository = productRepository;
            _employeesRepository = employeesRepository;
            _logged = logged;
        }

        public async Task<List<GetAllProductsExitQueriesResult>> Handle(GetAllProductsExitQueries request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            var movExits = await _repository.GetAllAsync();

            if (!movExits.Any())
            {
                throw new BadRequestException("Não há movimentações de saída cadastradas.");
            }

            return movExits.Select(movExits => new GetAllProductsExitQueriesResult(movExits)).ToList();
        }

    }
}
