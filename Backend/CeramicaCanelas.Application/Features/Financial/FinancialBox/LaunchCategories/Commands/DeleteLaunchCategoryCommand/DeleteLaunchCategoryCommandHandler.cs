using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.LaunchCategories.Commands.DeleteLaunchCategoryCommand
{
    public class DeleteLaunchCategoryCommandHandler : IRequestHandler<DeleteLaunchCategoryCommand, Unit>
    {
        private readonly ILaunchCategoryRepository _categoryRepository;
        private readonly ILogged _logged;

        public DeleteLaunchCategoryCommandHandler(ILaunchCategoryRepository categoryRepository, ILogged logged)
        {
            _categoryRepository = categoryRepository;
            _logged = logged;
        }

        public async Task<Unit> Handle(DeleteLaunchCategoryCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            var category = await _categoryRepository.GetByIdAsync(request.Id);

            if (category == null || category.IsDeleted)
                throw new BadRequestException("Categoria de lançamento não encontrada ou já foi excluída.");

            category.IsDeleted = true;
            category.ModifiedOn = DateTime.UtcNow;

            await _categoryRepository.Update(category);

            return Unit.Value;
        }
    }
}
