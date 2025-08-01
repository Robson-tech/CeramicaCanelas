using CeramicaCanelas.Domain.Entities.Almoxarifado;

namespace CeramicaCanelas.Application.Contracts.Persistance.Repositories
{
    public interface IMovEntryProductsRepository : IBaseRepository<ProductEntry>
    {
        /// <summary>
        /// Gets all product entries from the database
        /// </summary>
        /// <returns>
        /// A list of <see cref="CeramicaCanelas.Domain.Entities.Almoxarifado.ProductEntry"/> entities
        /// </returns>
        Task<List<ProductEntry>> GetAllAsync();
        /// <summary>
        /// Gets a product entry by its ID
        /// </summary>
        /// <param name="id">
        /// The ID of the product entry to retrieve
        /// </param>
        /// <returns>
        /// The <see cref="CeramicaCanelas.Domain.Entities.Almoxarifado.ProductEntry"/> entity with the specified ID, or null if not found
        /// </returns>
        Task<ProductEntry?> GetByIdAsync(Guid? id);
    }
}
