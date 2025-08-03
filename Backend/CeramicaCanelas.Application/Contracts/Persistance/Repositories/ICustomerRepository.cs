using CeramicaCanelas.Domain.Entities.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Contracts.Persistance.Repositories
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        public Task<Customer?> GetByIdAsync(Guid id);

        public IQueryable<Customer> QueryAllWithIncludes();
        public Task<List<Customer>> GetAllAsync();
    }
}
