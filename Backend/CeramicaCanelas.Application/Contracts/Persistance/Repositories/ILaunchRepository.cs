using CeramicaCanelas.Domain.Entities.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Contracts.Persistance.Repositories
{
    public interface ILaunchRepository : IBaseRepository<Launch>
    {
        public Task<Launch?> GetByIdAsync(Guid id);

        public IQueryable<Launch> QueryAllWithIncludes();

        public Task<List<Launch>> GetAllAsync();

        public IQueryable<Launch> QueryAll();

    }
}
