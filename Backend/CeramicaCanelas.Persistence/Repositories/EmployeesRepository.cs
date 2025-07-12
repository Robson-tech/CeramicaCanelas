using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CeramicaCanelas.Persistence.Repositories
{
    public class EmployeesRepository(DefaultContext context) : BaseRepository<Employee>(context), IEmployeesRepository 
    {
        /// <summary>
        /// Gets all employees from the database
        /// </summary>
        /// <returns>
        /// A list of <see cref="Employee"/> entities
        /// </returns>
        public async Task<List<Employee>> GetAllAsync()
        {
            return await Context.Employees.ToListAsync();
        }
        /// <summary>
        /// Gets an employee by its ID
        /// </summary>
        /// <param name="id">
        /// The ID of the employee to retrieve
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/> entity with the specified ID, or null if not found
        /// </returns>
        public async Task<Employee?> GetByIdAsync(Guid id)
        {
            return await Context.Employees.FindAsync(id);
        }

    }
}
