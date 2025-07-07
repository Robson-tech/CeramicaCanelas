using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Contracts.Application.Services
{
    public interface ISend
    {
        public bool SendRecoveryEmail(string recipientEmail, string recoveryLink);

    }
}
