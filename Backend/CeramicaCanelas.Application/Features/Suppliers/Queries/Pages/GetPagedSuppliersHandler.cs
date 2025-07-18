using CeramicaCanelas.Persistence.Repositories;
using MediatR;


namespace CeramicaCanelas.Application.Features.Suppliers.Queries.Pages
{
    public class GetPagedSuppliersHandler : IRequestHandler<PagedRequestSupplier, PagedResultSupplier>
    {
        private readonly ISupplierRepository _supplierRepository;

        public GetPagedSuppliersHandler(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<PagedResultSupplier> Handle(PagedRequestSupplier request, CancellationToken cancellationToken)
        {
            var all = await _supplierRepository.GetAllSuppliersAsync();

            // Filtro
            if (!string.IsNullOrWhiteSpace(request.Search))
                all = all.Where(s => s.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase));

            // Ordenação
            all = request.OrderBy?.ToLower() switch
            {
                "email" => request.Ascending ? all.OrderBy(s => s.Email) : all.OrderByDescending(s => s.Email),
                _ => request.Ascending ? all.OrderBy(s => s.Name) : all.OrderByDescending(s => s.Name)
            };

            var totalItems = all.Count();

            var pagedItems = all
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new SupplierResult(s))
                .ToList();

            return new PagedResultSupplier
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                Items = pagedItems
            };
        }
    }
}
