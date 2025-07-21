using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.User.Queries.Pages
{
    public class GetPagedUsersRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
