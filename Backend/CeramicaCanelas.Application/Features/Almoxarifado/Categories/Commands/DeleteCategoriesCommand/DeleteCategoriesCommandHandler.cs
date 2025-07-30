using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;


namespace CeramicaCanelas.Application.Features.Almoxarifado.Categories.Commands.DeleteCategoriesCommand
{
    public class DeleteCategoriesCommandHandler : IRequestHandler<DeleteCategoriesCommand, Unit>
    {

        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogged _logged;

        public DeleteCategoriesCommandHandler (ICategoryRepository categoryRepository, ILogged logged)
        {
            _categoryRepository = categoryRepository;
            _logged = logged;   
        }

        public async Task<Unit> Handle(DeleteCategoriesCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();

            if (user == null)
            {
                throw new BadRequestException("Usuário não autorizado a deletar categorias.");
            }

            var category = await _categoryRepository.GetByIdAsync(request.Id);
            if (category == null)
            {
                throw new BadRequestException("Erro ao deletar categória.");
            }

            await _categoryRepository.Delete(category);
            return Unit.Value;
        }
    }
}
