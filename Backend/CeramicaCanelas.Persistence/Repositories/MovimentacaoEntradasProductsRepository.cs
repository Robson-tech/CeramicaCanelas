using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Persistence.Repositories
{
    public class MovimentacaoEntradasProductsRepository(DefaultContext context) : BaseRepository<ProductEntry>(context), IMovimentacaoEntradasProductsRepository
    {
        public async Task<List<Domain.Entities.ProductEntry>> GetAllAsync()
        {
            return await Context.ProductEntries.ToListAsync();

        }
        public async Task<Domain.Entities.ProductEntry?> GetByIdAsync(Guid id)
        {
            return await Context.ProductEntries.FindAsync(id);

        }


    }
}
