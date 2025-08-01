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
    public class MovEntryProductsRepository(DefaultContext context) : BaseRepository<ProductEntry>(context), IMovEntryProductsRepository
    {
        public async Task<List<ProductEntry>> GetAllAsync()
        {
            return await Context.ProductEntries.Include(e => e.Product).ThenInclude(e => e.Category).Include(e => e.User).ToListAsync();

        }
        public async Task<ProductEntry?> GetByIdAsync(Guid? id)
        {
            return await Context.ProductEntries.FindAsync(id);

        }


    }
}
