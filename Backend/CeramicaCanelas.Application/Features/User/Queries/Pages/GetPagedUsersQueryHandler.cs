using CeramicaCanelas.Application.Contracts.Infrastructure;
using CeramicaCanelas.Application.Features.Almoxarifado.Product.Queries.Pages;
using MediatR;


namespace CeramicaCanelas.Application.Features.User.Queries.Pages
{
    public class GetPagedUsersQueryHandler(IIdentityAbstractor identityAbstractor)
        : IRequestHandler<GetPagedUsersQuery, PagedUserResult<GetPagedUsersResult>>
    {

        private readonly IIdentityAbstractor _identityAbstractor = identityAbstractor;

        public async Task<PagedUserResult<GetPagedUsersResult>> Handle(GetPagedUsersQuery request, CancellationToken cancellationToken)
        {
            var (users, total) = await _identityAbstractor.GetPagedUsersAsync(request.Page, request.PageSize);

            var result = new List<GetPagedUsersResult>();

            foreach (var user in users)
            {
                var roles = await _identityAbstractor.GetUserRolesAsync(user);

                result.Add(new GetPagedUsersResult
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = roles.ToList()
                });
            }

            return new PagedUserResult<GetPagedUsersResult>(result, request.Page, request.PageSize, total);
        }

    }

}
