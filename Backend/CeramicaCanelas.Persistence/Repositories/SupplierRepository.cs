using CeramicaCanelas.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace CeramicaCanelas.Persistence.Repositories
{
    public class SupplierRepository(DefaultContext context) : BaseRepository<Supplier>(context), ISupplierRepository
    {
        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            return await Context.Suppliers.ToListAsync();
        }

        public async Task<Supplier?> GetByIdAsync(Guid id)
        {
            return await Context.Suppliers
                .Include(s => s.ProductEntries)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
