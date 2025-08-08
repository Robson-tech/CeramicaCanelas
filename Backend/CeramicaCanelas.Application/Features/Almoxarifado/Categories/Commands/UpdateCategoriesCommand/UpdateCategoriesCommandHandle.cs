using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;

namespace CeramicaCanelas.Application.Features.Almoxarifado.Categories.Commands.UpdateCategoriesCommand
{
    public class UpdateCategoriesCommandHandle : IRequestHandler<UpdateCategoriesCommand, Unit>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogged _logged;
        public UpdateCategoriesCommandHandle(ICategoryRepository categoryRepository, ILogged logged)
        {
            _categoryRepository = categoryRepository;
            _logged = logged;
        }
        public async Task<Unit> Handle(UpdateCategoriesCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            var category = await ValidateCategories(request, cancellationToken);

            category.Name = request.Name;
            category.Description = request.Description ?? "Sem descrição";
            category.ModifiedOn = DateTime.UtcNow;

            await _categoryRepository.Update(category);

            return Unit.Value;

        }

        private async Task<Domain.Entities.Almoxarifado.Categories> ValidateCategories(UpdateCategoriesCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id);

            if (category == null)
            {
                throw new BadRequestException("Categória não encontrada.");
            }

            var validator = new UpdateCategoriesCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult);
            }

            return category;     

        }
    }
}
