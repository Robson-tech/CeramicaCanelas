using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Entities.Financial;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Persistence.Repositories
{
    public class CustomerRepository(DefaultContext context) : BaseRepository<Customer>(context), ICustomerRepository
    {
        /// <summary>
        /// Gets all customers from the database
        /// </summary>
        /// <returns>
        /// A list of <see cref="Customer"/> entities
        /// </returns>
        public async Task<List<Customer>> GetAllAsync()
        {
            return await Context.Customers
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public IQueryable<Customer> QueryAllWithIncludes()
        {
            return Context.Customers
                .AsQueryable();
        }


        /// <summary>
        /// Gets a customer by its ID
        /// </summary>
        /// <param name="id">
        /// The ID of the customer to retrieve
        /// </param>
        /// <returns>
        /// The <see cref="Customer"/> entity with the specified ID, or null if not found
        /// </returns>
        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            return await Context.Customers.FindAsync(id);
        }
    }
}
