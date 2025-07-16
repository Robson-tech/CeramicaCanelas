using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Almoxarifado.Product.Queries.Pages
{
    public class GetPagedProductsQueryHandler : IRequestHandler<PagedRequest, PagedResult<GetAllProductsQueriesResult>>
    {
        private readonly IProductRepository _productRepository;

        public GetPagedProductsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PagedResult<GetAllProductsQueriesResult>> Handle(PagedRequest request, CancellationToken cancellationToken)
        {
            var items = await _productRepository.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.OrderBy,
                request.Ascending,
                request.Search,
                request.MinPrice,
                request.MaxPrice,
                request.CategoryId
            );

            var totalItems = await _productRepository.GetTotalCountAsync(
                request.Search,
                request.MinPrice,
                request.MaxPrice,
                request.CategoryId
            );

            return new PagedResult<GetAllProductsQueriesResult>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                Items = items.Select(p => new GetAllProductsQueriesResult(p)).ToList()
            };
        }
    }

}
