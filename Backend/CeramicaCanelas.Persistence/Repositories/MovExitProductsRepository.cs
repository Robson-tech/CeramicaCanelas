using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Entities;

namespace CeramicaCanelas.Persistence.Repositories
{
    public class MovExitProductsRepository(DefaultContext context) : BaseRepository<ProductExit>(context), IMovExitProductsRepository
    {

    }
}
