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

        // Caminho absoluto no servidor VPS onde as imagens serão salvas
        private const string PastaBaseVps = "/var/www/ceramicacanelas/almoxarifado/products/images";

        // Caminho público que será exposto no navegador
        private const string UrlBase = "https://api.ceramicacanelas.shop/almoxarifado/products/images/";

        public CreatedProductCommandHandler(IProductRepository productRepository, ILogged logged)
        {
            _productRepository = productRepository;
            _logged = logged;
        }

        public async Task<Unit> Handle(CreatedProductCommand request, CancellationToken cancellationToken)
        {
            // Valida se o usuário está autenticado
            var user = await _logged.UserLogged();
            if (user == null)
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            // Valida o conteúdo da requisição
            await ValidateProduct(request, cancellationToken);

            string? imageUrl = null;

            if (request.Imagem != null)
            {
                // Garante que o diretório exista
                Directory.CreateDirectory(PastaBaseVps); // CreateDirectory é seguro mesmo se já existir

                var nomeArquivo = $"{Guid.NewGuid()}_{request.Imagem.FileName}";
                var caminhoAbsoluto = Path.Combine(PastaBaseVps, nomeArquivo);

                // Salva o arquivo na VPS
                using var stream = new FileStream(caminhoAbsoluto, FileMode.Create);
                await request.Imagem.CopyToAsync(stream);

                // Cria URL pública de acesso à imagem
                imageUrl = $"{UrlBase}{nomeArquivo}";
            }
            else
            {
                // Se não houver imagem, define uma URL padrão ou nula
                var ImageDefault = "productDefault.jpg"; // Nome do arquivo padrão
                imageUrl = $"{UrlBase}{ImageDefault}"; // ou defina uma URL padrão se necessário
            }

            var product = request.AssignToProducts();
            product.ImageUrl = imageUrl;

            await _productRepository.CreateAsync(product, cancellationToken);

            return Unit.Value;
        }

        private async Task ValidateProduct(CreatedProductCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreatedProductCommandValidator();
            var result = await validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
                throw new BadRequestException(result);
        }
    }
}
