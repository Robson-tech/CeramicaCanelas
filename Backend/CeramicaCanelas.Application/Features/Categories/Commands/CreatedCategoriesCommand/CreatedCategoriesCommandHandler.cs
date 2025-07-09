using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;


namespace CeramicaCanelas.Application.Features.Categories.Commands.CreatedCategoriesCommand
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

            var category = request.AssignToCategories();
            category.ImageUrl = url;

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
