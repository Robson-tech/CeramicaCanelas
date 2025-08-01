using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.LaunchCategories.Commands.CreateLaunchCategoryCommand
{
    public class CreateLaunchCategoryCommandHandler : IRequestHandler<CreateLaunchCategoryCommand, Unit>
    {
        private readonly ILaunchCategoryRepository _categoryRepository;
        private readonly ILogged _logged;

        public CreateLaunchCategoryCommandHandler(ILaunchCategoryRepository categoryRepository, ILogged logged)
        {
            _categoryRepository = categoryRepository;
            _logged = logged;
        }

        public async Task<Unit> Handle(CreateLaunchCategoryCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            await ValidateCategory(request, cancellationToken);

            var category = request.AssignToEntity();

            await _categoryRepository.CreateAsync(category, cancellationToken);

            return Unit.Value;
        }

        private async Task ValidateCategory(CreateLaunchCategoryCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateLaunchCategoryCommandValidator();
            var result = await validator.ValidateAsync(request, cancellationToken);

            if (!result.IsValid)
                throw new BadRequestException(result);

        }
    }
}
