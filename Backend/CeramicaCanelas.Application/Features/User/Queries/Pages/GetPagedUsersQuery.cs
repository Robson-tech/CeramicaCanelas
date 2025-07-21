using CeramicaCanelas.Application.Features.Almoxarifado.Product.Queries.Pages;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.User.Queries.Pages
{
    public class GetPagedUsersQuery : GetPagedUsersRequest, IRequest<PagedUserResult<GetPagedUsersResult>>
    {
    }
}
