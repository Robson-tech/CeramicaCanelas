using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Entities.Almoxarifado;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Persistence.Repositories
{
    public class ProductRepository(DefaultContext context) : BaseRepository<Products>(context), IProductRepository
    {
        public async Task<IEnumerable<Products>> GetAllProductsAsync()
        {
            return await Context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Products> GetProductByIdAsync(Guid? id)
        {
            return await Context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id) 
                ?? throw new KeyNotFoundException("Produto não encontrado.");
        }

        public async Task<IEnumerable<Products>> GetProductsByCategoryIdAsync(Guid? categoryId)
        {
            return await Context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<List<Products>> GetPagedAsync(
            int page, int pageSize, string? orderBy, bool ascending,
            string? search, float? minPrice, float? maxPrice, Guid? categoryId)
        {
            var query = Context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // 🔎 Busca case-insensitive com ToLower()
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(p => (p.Name ?? string.Empty).ToLower().Contains(s));
            }

            if (minPrice.HasValue)
                query = query.Where(p => p.ValueTotal >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.ValueTotal <= maxPrice.Value);

            if (categoryId.HasValue && categoryId.Value != Guid.Empty)
                query = query.Where(p => p.CategoryId == categoryId);

            query = orderBy?.ToLower() switch
            {
                "name" => ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                "price" => ascending ? query.OrderBy(p => p.ValueTotal) : query.OrderByDescending(p => p.ValueTotal),
                _ => query.OrderBy(p => p.Name)
            };

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string? search, float? minPrice, float? maxPrice, Guid? categoryId)
        {
            var query = Context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search));

            if (minPrice.HasValue)
                query = query.Where(p => p.ValueTotal >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.ValueTotal <= maxPrice.Value);

            if (categoryId.HasValue && categoryId.Value != Guid.Empty)
                query = query.Where(p => p.CategoryId == categoryId);

            return await query.CountAsync();
        }
    }
}
