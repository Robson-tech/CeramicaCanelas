using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Domain.Exception
{
    public class ForbiddenException : ApiException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;

        public ForbiddenException(string error) : base(error) { }
    }
}
