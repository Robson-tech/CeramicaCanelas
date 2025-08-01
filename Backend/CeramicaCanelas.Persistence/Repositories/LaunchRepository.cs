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
    public class LaunchRepository(DefaultContext context) : BaseRepository<Launch>(context), ILaunchRepository
    {
        public async Task<Launch?> GetByIdAsync(Guid id)
        {
            return await Context.Launches.FindAsync(id);
        }

        public async Task<List<Launch>> GetAllAsync()
        {
            return await Context.Launches.ToListAsync();
        }
    }
}
