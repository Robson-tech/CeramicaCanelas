using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CeramicaCanelas.Application.Features.Almoxarifado.Movimentacoes_ES.Entradas.Queries.Pages
{
    public class GetPagedProductsEntryHandler : IRequestHandler<PagedRequestProductsEntry, PagedResultProductsEntry>
    {
        private readonly IMovEntryProductsRepository _entryRepository;

        public GetPagedProductsEntryHandler(IMovEntryProductsRepository entryRepository)
        {
            _entryRepository = entryRepository;
        }

        public async Task<PagedResultProductsEntry> Handle(PagedRequestProductsEntry request, CancellationToken cancellationToken)
        {
            var all = await _entryRepository.GetAllAsync();

            // Filtro por nome do produto
            if (!string.IsNullOrWhiteSpace(request.Search))
                all = all.Where(e => e.Product.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase)).ToList();

            // Filtro por categoria
            if (request.CategoryId.HasValue && request.CategoryId != Guid.Empty)
                all = all.Where(e => e.Product.CategoryId == request.CategoryId).ToList();

            // Ordenação
            all = request.OrderBy?.ToLower() switch
            {
                "productname" => request.Ascending
                    ? all.OrderBy(e => e.Product.Name).ToList()
                    : all.OrderByDescending(e => e.Product.Name).ToList(),

                "date" => request.Ascending
                    ? all.OrderBy(e => e.EntryDate).ToList()
                    : all.OrderByDescending(e => e.EntryDate).ToList(),

                _ => all.OrderByDescending(e => e.EntryDate).ToList()
            };

            var totalItems = all.Count;

            var pagedItems = all
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(entry => new EntryItemResult(entry)) // Aqui usamos o DTO do item
                .ToList();

            return new PagedResultProductsEntry
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                Items = pagedItems
            };
        }
    }
}
