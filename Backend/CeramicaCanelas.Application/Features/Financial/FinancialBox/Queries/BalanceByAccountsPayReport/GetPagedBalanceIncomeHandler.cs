using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            var query = _launchRepository.QueryAllWithIncludes()
                .Where(l => l.Type == LaunchType.Income && l.Status == PaymentStatus.Paid);

            // Aplica filtros somente se as datas forem fornecidas
            if (request.StartDate.HasValue)
                query = query.Where(l => l.LaunchDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                query = query.Where(l => l.LaunchDate <= request.EndDate.Value);

            // Pega as datas mínima e máxima dos lançamentos filtrados
            var minDate = await query.MinAsync(l => (DateOnly?)l.LaunchDate, cancellationToken);
            var maxDate = await query.MaxAsync(l => (DateOnly?)l.LaunchDate, cancellationToken);

            // Soma total de receitas no período
            var totalIncomeOverall = await query.SumAsync(l => l.Amount, cancellationToken);

            // Agrupa por método de pagamento
            var grouped = await query
                .GroupBy(l => l.PaymentMethod)
                .Select(g => new BalanceIncomeResult
                {
                    PaymentMethod = g.Key.ToString(),
                    TotalIncome = g.Sum(l => l.Amount)
                })
                .ToListAsync(cancellationToken);

            // Filtro textual pós-agrupamento (corrigido)
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchLower = request.Search.ToLower();
                grouped = grouped
                    .Where(g => g.PaymentMethod.ToLower().Contains(searchLower))
                    .ToList();
            }

            var totalItems = grouped.Count;

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
                Items = pagedItems,
                TotalIncomeOverall = totalIncomeOverall,
                StartDate = request.StartDate ?? minDate,
                EndDate = request.EndDate ?? maxDate
            };
        }
    }
}
