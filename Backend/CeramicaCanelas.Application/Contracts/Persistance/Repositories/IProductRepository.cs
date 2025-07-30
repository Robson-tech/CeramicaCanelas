using CeramicaCanelas.Domain.Entities.Almoxarifado;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Contracts.Persistance.Repositories
{
    public interface IProductRepository : IBaseRepository<Products>
    {
        public Task<IEnumerable<Products>> GetAllProductsAsync();

        public Task<Products> GetProductByIdAsync(Guid? id);

        public Task<IEnumerable<Products>> GetProductsByCategoryIdAsync(Guid? categoryId);

        Task<List<Products>> GetPagedAsync(int page, int pageSize, string? orderBy, bool ascending, string? search, float? minPrice, float? maxPrice, Guid? categoryId);
        Task<int> GetTotalCountAsync(string? search, float? minPrice, float? maxPrice, Guid? categoryId);

    }
}
