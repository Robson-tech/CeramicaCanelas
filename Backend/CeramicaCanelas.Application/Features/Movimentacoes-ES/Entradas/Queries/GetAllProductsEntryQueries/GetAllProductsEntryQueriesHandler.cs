using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Application.Features.Employees.Queries.GetAllEmployeesQueries;
using CeramicaCanelas.Domain.Exception;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Movimentacoes_ES.Entradas.Queries.GetAllProductsEntryQueries
{
    public class GetAllProductsEntryQueries : IRequest<List<GetAllProductsEntryQueriesResult>>
    {
        // Add any parameters needed for the query here
    }
    public class GetAllProductsEntryQueriesHandler : IRequestHandler<GetAllProductsEntryQueries, List<GetAllProductsEntryQueriesResult>>
    {
        private readonly IMovEntryProductsRepository _movimentacaoEntradasProductsRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogged _logged;

        public GetAllProductsEntryQueriesHandler(IMovEntryProductsRepository movimentacaoEntradasProductsRepository, ILogged logged, IUserRepository userRepository)
        {
            _movimentacaoEntradasProductsRepository = movimentacaoEntradasProductsRepository;
            _logged = logged;
            _userRepository = userRepository;
        }

        public async Task<List<GetAllProductsEntryQueriesResult>> Handle(GetAllProductsEntryQueries request, CancellationToken cancellationToken)
        {
            if (await _logged.UserLogged() == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            var productEntries = await _movimentacaoEntradasProductsRepository.GetAllAsync();
            if (!productEntries.Any())
            {
                throw new BadRequestException("Não há entradas de produtos cadastradas.");
            }

            return productEntries.Select(productEntries => new GetAllProductsEntryQueriesResult(productEntries)).ToList();
        }

    }
}
