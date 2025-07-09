using CeramicaCanelas.Domain.Entities;

namespace CeramicaCanelas.Application.Contracts.Persistance.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Domain.Entities.Categories>
    {
        public Task<Categories?> GetByIdAsync(Guid id);

        public Task<List<Categories>> GetAllAsync();


    }
}
