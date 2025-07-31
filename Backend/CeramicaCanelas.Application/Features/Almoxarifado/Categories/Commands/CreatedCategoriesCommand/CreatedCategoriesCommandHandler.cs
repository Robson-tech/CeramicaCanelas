using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;


namespace CeramicaCanelas.Application.Features.Almoxarifado.Categories.Commands.CreatedCategoriesCommand
{
    public class CreatedCategoriesCommandHandler : IRequestHandler<CreatedCategoriesCommand, Unit>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogged _logged;

        public CreatedCategoriesCommandHandler(
            ICategoryRepository categoryRepository,
            ILogged logged)
        {
            _categoryRepository = categoryRepository;
            _logged = logged;
        }

        public async Task<Unit> Handle (CreatedCategoriesCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado");
            }
            
            await ValidateCategories(request, cancellationToken);

            var category = request.AssignToCategories();

            await _categoryRepository.CreateAsync(category, cancellationToken);

            return Unit.Value;
        }

        private async Task ValidateCategories (CreatedCategoriesCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreatedCategoriesCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult);
            }
        }
    }
}
