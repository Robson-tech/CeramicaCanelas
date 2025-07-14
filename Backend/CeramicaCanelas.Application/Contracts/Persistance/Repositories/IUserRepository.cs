using CeramicaCanelas.Domain.Entities;

namespace CeramicaCanelas.Application.Contracts.Persistance.Repositories;

public interface IUserRepository : IBaseRepository<User> 
{

    public Task<User?> GetUserById(string id);
}
