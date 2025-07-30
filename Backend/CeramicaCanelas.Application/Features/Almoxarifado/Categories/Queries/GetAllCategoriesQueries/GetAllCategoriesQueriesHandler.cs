using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Entities;
using CeramicaCanelas.Domain.Exception;
using MediatR;

namespace CeramicaCanelas.Application.Features.Almoxarifado.Categories.Queries.GetAllCategoriesQueries
{
    public class GetAllCategoriesQueries : IRequest<List<GetAllCategoriesResult>> { } // Mudança no retorno

    public class GetAllCategoriesQueriesHandler : IRequestHandler<GetAllCategoriesQueries, List<GetAllCategoriesResult>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogged _logged;

        public GetAllCategoriesQueriesHandler(ICategoryRepository categoryRepository, ILogged logged)
        {
            _categoryRepository = categoryRepository;
            _logged = logged;
        }

        public async Task<List<GetAllCategoriesResult>> Handle(GetAllCategoriesQueries request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAllAsync();

            if (!categories.Any())
            {
                throw new BadRequestException("Não há categórias cadastradas.");
            }

            return categories.Select(product => new GetAllCategoriesResult(product)).ToList();

        }


    }
}
