using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.BalanceByAccountsPayReport
{
    public class GetPagedBalanceIncomeHandler : IRequestHandler<PagedRequestBalanceIncome, PagedResultBalanceIncome>
    {
        private readonly ILaunchRepository _launchRepository;

        public GetPagedBalanceIncomeHandler(ILaunchRepository launchRepository)
        {
            _launchRepository = launchRepository;
        }

        public async Task<PagedResultBalanceIncome> Handle(PagedRequestBalanceIncome request, CancellationToken cancellationToken)
        {
            var launches = await _launchRepository.GetAllAsync();

            var filtered = launches
                .Where(l => l.Type == LaunchType.Income)
                .AsQueryable();

            if (request.StartDate.HasValue)
                filtered = filtered.Where(l => l.LaunchDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                filtered = filtered.Where(l => l.LaunchDate <= request.EndDate.Value);

            var grouped = filtered
                .ToList() // LINQ in-memory para permitir uso de .ToString()
                .GroupBy(l => l.PaymentMethod.ToString())
                .Select(g => new BalanceIncomeResult
                {
                    PaymentMethod = g.Key,
                    TotalIncome = g.Sum(l => l.Amount)
                });

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                grouped = grouped
                    .Where(g => g.PaymentMethod.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
            }

            var totalItems = grouped.Count();

            var pagedItems = grouped
                .OrderBy(g => g.PaymentMethod)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PagedResultBalanceIncome
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                Items = pagedItems
            };
        }
    }
}
