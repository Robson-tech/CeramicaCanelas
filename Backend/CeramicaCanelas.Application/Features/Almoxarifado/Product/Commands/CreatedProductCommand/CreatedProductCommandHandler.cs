using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;


namespace CeramicaCanelas.Application.Features.Almoxarifado.Product.Commands.CreatedProductCommand
{
    public class CreatedProductCommandHandler : IRequestHandler<CreatedProductCommand, Unit>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogged _logged;
        public CreatedProductCommandHandler(IProductRepository productRepository, ILogged logged)
        {
            _productRepository = productRepository;
            _logged = logged;
        }
        public async Task<Unit> Handle(CreatedProductCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            await ValidateProduct(request, cancellationToken);

            var pasta = Path.Combine("wwwroot", "products", "images");
            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            const string UrlBase = "https://localhost:7014/products/images/";
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

            var product = request.AssignToProducts();
            product.ImageUrl = url;

            await _productRepository.CreateAsync(product, cancellationToken);
            return Unit.Value;
        }

        private async Task ValidateProduct(CreatedProductCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreatedProductCommandValidator();
            var result = await validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
            {
                throw new BadRequestException(result);
            }
        }
    }
}
