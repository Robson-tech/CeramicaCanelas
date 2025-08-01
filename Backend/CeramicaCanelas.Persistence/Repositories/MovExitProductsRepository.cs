using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Entities.Almoxarifado;
using Microsoft.EntityFrameworkCore;

namespace CeramicaCanelas.Persistence.Repositories
{
    public class MovExitProductsRepository(DefaultContext context) : BaseRepository<ProductExit>(context), IMovExitProductsRepository
    {

        public async Task<ProductExit?> GetByIdAsync(Guid id)
        {
            return await Context.ProductExits.FindAsync(id);
        }

        public async Task<List<ProductExit>> GetAllAsync()
        {
            return await Context.ProductExits.Include(e => e.Employee).Include(e => e.Product).Include(e => e.User).ToListAsync();
        }

    }
}
