using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Persistence.Repositories
{
    public class ProductRepository(DefaultContext context) : BaseRepository<Domain.Entities.Products>(context), IProductRepository
    {
        public async Task<IEnumerable<Domain.Entities.Products>> GetAllProductsAsync()
        {
            return await Context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Domain.Entities.Products> GetProductByIdAsync(Guid id)
        {
            return await Context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id) 
                ?? throw new KeyNotFoundException("Produto não encontrado.");
        }

        public async Task<IEnumerable<Domain.Entities.Products>> GetProductsByCategoryIdAsync(Guid categoryId)
        {
            return await Context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .ToListAsync();
        }
    }
}
