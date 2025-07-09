using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Contracts.Application.Services
{
    public interface ILogged
    {
        Task<Domain.Entities.User> UserLogged();
        Task<bool> IsInRole(string role);
    }
}
