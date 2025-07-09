using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Application.Features.Categories.Commands.CreatedCategoriesCommand;
using CeramicaCanelas.Domain.Exception;
using MediatR;

namespace CeramicaCanelas.Application.Features.Categories.Commands.UpdateCategoriesCommand
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

            var pasta = Path.Combine("wwwroot", "categories", "images");
            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            const string UrlBase = "https://localhost:7014/categories/images/";
            string? url = null;

            if (request.Imagem != null)
            {
                var nomeArquivo = $"{Guid.NewGuid()}_{request.Imagem.FileName}";
                var caminho = Path.Combine(pasta, nomeArquivo);

                using (var stream = new FileStream(caminho, FileMode.Create))
                {
                    await request.Imagem.CopyToAsync(stream);
                }

                url = $"{UrlBase}{nomeArquivo}";
            }

            category.Name = request.Name;
            category.Description = request.Description;
            category!.ImageUrl = url;
            category.ModifiedOn = DateTime.UtcNow;

            await _categoryRepository.Update(category);

            return Unit.Value;

        }

        private async Task<Domain.Entities.Categories> ValidateCategories(UpdateCategoriesCommand request, CancellationToken cancellationToken)
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
