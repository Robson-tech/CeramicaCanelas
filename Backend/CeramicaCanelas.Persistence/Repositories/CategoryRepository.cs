using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Entities.Almoxarifado;
using CeramicaCanelas.Domain.Entities.Financial;
using Microsoft.EntityFrameworkCore;


namespace CeramicaCanelas.Persistence.Repositories
{
    public class CategoryRepository(DefaultContext context) : BaseRepository<Categories>(context), ICategoryRepository
    {
        /// <summary>
        /// Gets all categories from the database
        /// </summary>
        /// <returns>
        /// A list of <see cref="Categories"/> entities
        /// </returns>
        public async Task<List<Categories>> GetAllAsync()
        {
            return await Context.Categories.ToListAsync();
        }
        /// <summary>
        /// Gets a category by its ID
        /// </summary>
        /// <param name="id">
        /// The ID of the category to retrieve
        /// </param>
        /// <returns>
        /// The <see cref="Categories"/> entity with the specified ID, or null if not found
        /// </returns>
        public async Task<Categories?> GetByIdAsync(Guid id)
        {
            return await Context.Categories.FindAsync(id);
        }

    }
}
