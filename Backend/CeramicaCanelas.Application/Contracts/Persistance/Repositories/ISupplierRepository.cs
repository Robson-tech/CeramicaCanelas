
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Entities;

namespace CeramicaCanelas.Persistence.Repositories
{
    public interface ISupplierRepository : IBaseRepository<Domain.Entities.Supplier>
    {
        public Task<IEnumerable<Supplier>> GetAllSuppliersAsync();

        public Task<Supplier?> GetByIdAsync(Guid id);


    }
}