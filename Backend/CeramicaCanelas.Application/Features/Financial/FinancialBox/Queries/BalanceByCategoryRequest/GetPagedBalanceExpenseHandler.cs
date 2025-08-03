using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;

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
            var launches = _launchRepository.QueryAllWithIncludes();

            var filtered = launches
                .Where(l => l.Type == LaunchType.Expense && l.Status == PaymentStatus.Paid)
                .AsQueryable();

            if (request.StartDate.HasValue)
                filtered = filtered.Where(l => l.LaunchDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                filtered = filtered.Where(l => l.LaunchDate <= request.EndDate.Value);

            var grouped = filtered
                .ToList()
                .GroupBy(l => l.Category?.Name ?? "Sem categoria")
                .Select(g => new BalanceExpenseResult
                {
                    CategoryName = g.Key,
                    TotalExpense = g.Sum(l => l.Amount)
                });


            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                grouped = grouped
                    .Where(g => g.CategoryName.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
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
