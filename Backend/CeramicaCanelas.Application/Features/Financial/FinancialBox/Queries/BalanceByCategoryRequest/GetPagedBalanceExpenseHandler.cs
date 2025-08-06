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

            // Aplica filtros somente se as datas forem fornecidas
            if (request.StartDate.HasValue)
                query = query.Where(l => l.LaunchDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                query = query.Where(l => l.LaunchDate <= request.EndDate.Value);

            // Obtém a menor e maior data real da consulta filtrada
            var minDate = await query.MinAsync(l => (DateOnly?)l.LaunchDate, cancellationToken);
            var maxDate = await query.MaxAsync(l => (DateOnly?)l.LaunchDate, cancellationToken);

            // Soma total de despesas
            var totalExpenseOverall = await query.SumAsync(l => l.Amount, cancellationToken);

            // Agrupamento por nome da categoria
            var grouped = await query
                .GroupBy(l => l.Category!.Name ?? "Sem categoria")
                .Select(g => new BalanceExpenseResult
                {
                    CategoryName = g.Key,
                    TotalExpense = g.Sum(l => l.Amount)
                })
                .ToListAsync(cancellationToken);

            // Filtro textual após o agrupamento (corrigido para compatibilidade)
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchLower = request.Search.ToLower();
                grouped = grouped
                    .Where(g => g.CategoryName.ToLower().Contains(searchLower))
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
                Items = pagedItems,
                TotalExpenseOverall = totalExpenseOverall,
                StartDate = request.StartDate ?? minDate,
                EndDate = request.EndDate ?? maxDate
            };
        }
    }
}
