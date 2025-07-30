using CeramicaCanelas.Domain.Entities.Almoxarifado;

namespace CeramicaCanelas.Application.Contracts.Persistance.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Categories>
    {
        public Task<Categories?> GetByIdAsync(Guid id);

        public Task<List<Categories>> GetAllAsync();


    }
}
