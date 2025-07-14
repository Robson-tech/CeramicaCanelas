using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CeramicaCanelas.Persistence.Repositories;

public class UserRepository(DefaultContext context): BaseRepository<User>(context), IUserRepository
{
    public async Task<User?> GetUserById(string id)
    {
        return await Context.Users.FindAsync(id);
    }

}
