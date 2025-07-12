namespace CeramicaCanelas.Application.Contracts.Persistance.Repositories
{
    public interface IEmployeesRepository : IBaseRepository<Domain.Entities.Employee>
    {
        /// <summary>
        /// Gets all employees from the database
        /// </summary>
        /// <returns>
        /// A list of <see cref="Domain.Entities.Employees"/> entities
        /// </returns>
        Task<List<Domain.Entities.Employee>> GetAllAsync();
        /// <summary>
        /// Gets an employee by its ID
        /// </summary>
        /// <param name="id">
        /// The ID of the employee to retrieve
        /// </param>
        /// <returns>
        /// The <see cref="Domain.Entities.Employee"/> entity with the specified ID, or null if not found
        /// </returns>
        Task<Domain.Entities.Employee?> GetByIdAsync(Guid id);
    }
}
