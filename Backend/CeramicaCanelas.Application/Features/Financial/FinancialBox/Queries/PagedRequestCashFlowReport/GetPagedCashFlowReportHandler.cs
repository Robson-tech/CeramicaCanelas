using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PagedRequestCashFlowReport
{
    public class GetPagedCashFlowReportHandler
        : IRequestHandler<PagedRequestCashFlowReport, PagedResultCashFlowReport>
    {
        private readonly ILaunchRepository _launchRepository;

        public GetPagedCashFlowReportHandler(ILaunchRepository launchRepository)
        {
            _launchRepository = launchRepository;
        }

        public async Task<PagedResultCashFlowReport> Handle(
            PagedRequestCashFlowReport request,
            CancellationToken ct)
        {
            // Base SEM includes (projeção resolve Category/Customer)
            var baseQuery = _launchRepository.QueryAll()
                .AsNoTracking()
                .Where(l => l.Status == PaymentStatus.Paid);

            // Período
            if (request.StartDate.HasValue)
                baseQuery = baseQuery.Where(l => l.LaunchDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                baseQuery = baseQuery.Where(l => l.LaunchDate <= request.EndDate.Value);

            // Busca por descrição (PostgreSQL: ILIKE)
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search;
                baseQuery = baseQuery.Where(l => l.Description != null && l.Description.Contains(s));
            }

            // Filtro por tipo
            var filteredQuery = baseQuery;
            if (request.type == LaunchType.Income)
                filteredQuery = filteredQuery.Where(l => l.Type == LaunchType.Income);
            else if (request.type == LaunchType.Expense)
                filteredQuery = filteredQuery.Where(l => l.Type == LaunchType.Expense);

            // ===== Totais (um registro por Id) =====
            var distinctBase = baseQuery
                .Select(l => new { l.Id, l.Amount, l.Type })
                .GroupBy(x => x.Id)
                .Select(g => g.First());

            var totalEntradas = await distinctBase
                .Where(x => x.Type == LaunchType.Income)
                .SumAsync(x => x.Amount, ct);

            var totalSaidas = await distinctBase
                .Where(x => x.Type == LaunchType.Expense)
                .SumAsync(x => x.Amount, ct);

            // Total de itens respeitando o filtro de tipo (Ids distintos)
            var totalItems = await filteredQuery
                .Select(l => l.Id)
                .Distinct()
                .CountAsync(ct);

            // ===== Paginação por Id com ordenação estável =====
            // Ordena por LaunchDate “por lançamento” (caso haja múltiplas linhas por Id)
            var pageIds = await filteredQuery
                .Select(l => new { l.Id, l.LaunchDate })
                .GroupBy(x => x.Id)
                .Select(g => new { Id = g.Key, LastDate = g.Max(x => x.LaunchDate) })
                .OrderByDescending(x => x.LastDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => x.Id)
                .ToListAsync(ct);

            // ===== Itens da página =====
            var items = await baseQuery
                .Where(l => pageIds.Contains(l.Id))
                .Select(l => new CashFlowReportItem
                {
                    LaunchDate = l.LaunchDate,
                    Description = l.Description ?? string.Empty,
                    Amount = l.Amount,
                    Type = l.Type,
                    CategoryName = l.Category != null ? l.Category.Name : "Sem categoria",
                    CustomerName = l.Customer != null ? l.Customer.Name : "Sem cliente",
                    PaymentMethod = l.PaymentMethod.ToString()
                })
                .OrderByDescending(i => i.LaunchDate)
                .ToListAsync(ct);

            return new PagedResultCashFlowReport
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalEntradas = totalEntradas,
                TotalSaidas = totalSaidas,
                Items = items
            };
        }
    }
}
