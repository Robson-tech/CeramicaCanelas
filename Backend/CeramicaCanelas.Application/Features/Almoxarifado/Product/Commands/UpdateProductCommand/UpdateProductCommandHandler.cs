using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Entities.Almoxarifado;
using CeramicaCanelas.Domain.Exception;
using MediatR;

namespace CeramicaCanelas.Application.Features.Almoxarifado.Product.Commands.UpdateProductCommand
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogged _logged;
        private readonly ICategoryRepository _categoryRepository;

        public UpdateProductCommandHandler(IProductRepository productRepository, ILogged logged, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _logged = logged;
            _categoryRepository = categoryRepository;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            var product = await ValidateUpdateProduct(request, cancellationToken);

            // NOVO CAMINHO NO SERVIDOR VPS
            var pasta = Path.Combine("/var/www/ceramicacanelas/almoxarifado/products/images");
            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            // NOVA URL BASE
            const string UrlBase = "https://api.ceramicacanelas.shop/almoxarifado/products/images/";
            string? url = product.ImageUrl; // Mantém a imagem anterior se não for atualizada

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

            // Atualiza os dados
            product.Name = request.Name;
            product.Code = request.Code;
            product.UnitOfMeasure = request.UnitOfMeasure;
            product.StockInitial = request.StockInitial;
            product.StockMinium = request.StockMinium;
            product.ValueTotal = request.Value;
            product.ImageUrl = url;
            product.IsReturnable = request.IsReturnable;
            product.Observation = request.Observation;
            product.CategoryId = request.CategoryId ?? product.CategoryId;
            product.ModifiedOn = DateTime.UtcNow;

            // Verifica se a categoria existe, se for informada
            if (request.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value);
                if (category == null)
                    throw new BadRequestException("Categoria informada não existe.");
            }

            await _productRepository.Update(product);
            return Unit.Value;

        }

        private async Task<Products> ValidateUpdateProduct(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateProductCommandValidator();
            var result = await validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
                throw new BadRequestException(result);

            var product = await _productRepository.GetProductByIdAsync(request.Id);
            if (product == null)
                throw new BadRequestException("Produto não encontrado.");

            return product;
        }
    }
}
