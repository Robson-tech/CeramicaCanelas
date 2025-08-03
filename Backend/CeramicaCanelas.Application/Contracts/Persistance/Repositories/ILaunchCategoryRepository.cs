using CeramicaCanelas.Domain.Entities.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Contracts.Persistance.Repositories
{
    public interface ILaunchCategoryRepository : IBaseRepository<LaunchCategory>
    {
        public Task<LaunchCategory?> GetByIdAsync(Guid id);

        public IQueryable<LaunchCategory> QueryAllWithIncludes();

        public Task<List<LaunchCategory>> GetAllAsync();
    }
}
