using CeramicaCanelas.Domain.Entities.Almoxarifado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Contracts.Persistance.Repositories
{
    public interface IMovExitProductsRepository : IBaseRepository<ProductExit>
    {
        public Task<ProductExit?> GetByIdAsync(Guid id);

        public Task<List<ProductExit>> GetAllAsync();


    }
}
