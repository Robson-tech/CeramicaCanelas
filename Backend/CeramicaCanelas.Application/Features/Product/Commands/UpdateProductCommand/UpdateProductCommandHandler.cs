using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Exception;
using MediatR;


namespace CeramicaCanelas.Application.Features.Product.Commands.UpdateProductCommand
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogged _logged;
        public UpdateProductCommandHandler(IProductRepository productRepository, ILogged logged)
        {
            _productRepository = productRepository;
            _logged = logged;
        }
        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var user = await _logged.UserLogged();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }
            var product = await ValidateUpdateProduct(request, cancellationToken);
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
            product.Name = request.Name;
            product.Code = request.Code;
            product.UnitOfMeasure = request.UnitOfMeasure;
            product.StockInitial = request.StockInitial;
            product.StockMinium = request.StockMinium;
            product.ValueTotal = request.Value;
            product.ImageUrl = url;
            product.IsReturnable = request.IsReturnable;
            product.Observation = request.Observation;
            product.CategoryId = request.CategoryId;
            product.ModifiedOn = DateTime.UtcNow;
            await _productRepository.Update(product);
            return Unit.Value;
        }
        private async Task<Domain.Entities.Products> ValidateUpdateProduct(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateProductCommandValidator();
            var result = await validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
            {
                throw new BadRequestException(result);
            }

            var product = await _productRepository.GetProductByIdAsync(request.Id);

            if (product == null)
            {
                throw new BadRequestException("Produto não encontrado.");
            }

            return product;
        }
    }
}
