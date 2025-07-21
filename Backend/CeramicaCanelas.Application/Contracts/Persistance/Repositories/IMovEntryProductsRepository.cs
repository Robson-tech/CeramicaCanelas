namespace CeramicaCanelas.Application.Contracts.Persistance.Repositories
{
    public interface IMovEntryProductsRepository : IBaseRepository<Domain.Entities.ProductEntry>
    {
        /// <summary>
        /// Gets all product entries from the database
        /// </summary>
        /// <returns>
        /// A list of <see cref="Domain.Entities.ProductEntry"/> entities
        /// </returns>
        Task<List<Domain.Entities.ProductEntry>> GetAllAsync();
        /// <summary>
        /// Gets a product entry by its ID
        /// </summary>
        /// <param name="id">
        /// The ID of the product entry to retrieve
        /// </param>
        /// <returns>
        /// The <see cref="Domain.Entities.ProductEntry"/> entity with the specified ID, or null if not found
        /// </returns>
        Task<Domain.Entities.ProductEntry?> GetByIdAsync(Guid? id);
    }
}
