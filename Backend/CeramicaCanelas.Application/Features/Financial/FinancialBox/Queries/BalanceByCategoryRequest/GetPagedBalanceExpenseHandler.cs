using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.BalanceByCategoryRequest
{
    public class GetPagedBalanceExpenseHandler : IRequestHandler<PagedRequestBalanceExpense, PagedResultBalanceExpense>
    {
        private readonly ILaunchRepository _launchRepository;

        public GetPagedBalanceExpenseHandler(ILaunchRepository launchRepository)
        {
            _launchRepository = launchRepository;
        }

        public async Task<PagedResultBalanceExpense> Handle(PagedRequestBalanceExpense request, CancellationToken cancellationToken)
        {
            var query = _launchRepository.QueryAllWithIncludes()
                .Where(l => l.Type == LaunchType.Expense && l.Status == PaymentStatus.Paid);

            var groupedQuery = query;

            if (request.StartDate.HasValue)
                groupedQuery = groupedQuery.Where(l => l.LaunchDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                groupedQuery = groupedQuery.Where(l => l.LaunchDate < request.EndDate.Value.AddDays(1));

            var grouped = await groupedQuery
                .GroupBy(l => l.Category!.Name ?? "Sem categoria")
                .Select(g => new BalanceExpenseResult
                {
                    CategoryName = g.Key,
                    TotalExpense = g.Sum(l => l.Amount)
                })
                .ToListAsync(cancellationToken);

            // Filtro textual após trazer os dados agrupados (na memória)
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                grouped = grouped
                    .Where(g => g.CategoryName.Contains(request.Search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }


            var totalItems = grouped.Count();

            var pagedItems = grouped
                .OrderBy(g => g.CategoryName)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PagedResultBalanceExpense
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                Items = pagedItems
            };
        }
    }
}
