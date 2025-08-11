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
            // Sanitiza paginação
            var page = request.Page <= 0 ? 1 : request.Page;
            var size = request.PageSize <= 0 ? 10 : request.PageSize;

            // Base SEM includes (projeção resolve Category/Customer)
            var baseQuery = _launchRepository.QueryAll()
                .AsNoTracking()
                .Where(l => l.Status == PaymentStatus.Paid);

            // Período
            if (request.StartDate.HasValue)
                baseQuery = baseQuery.Where(l => l.LaunchDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                baseQuery = baseQuery.Where(l => l.LaunchDate <= request.EndDate.Value);

            // Busca por descrição
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search;
                baseQuery = baseQuery.Where(l => l.Description != null && l.Description.Contains(s));
                // Postgres case-insensitive (opcional):
                // baseQuery = baseQuery.Where(l => EF.Functions.ILike(l.Description ?? "", $"%{s}%"));
            }

            // Filtro por tipo
            var filteredQuery = baseQuery;
            if (request.type == LaunchType.Income)
                filteredQuery = filteredQuery.Where(l => l.Type == LaunchType.Income);
            else if (request.type == LaunchType.Expense)
                filteredQuery = filteredQuery.Where(l => l.Type == LaunchType.Expense);

            // ===== Totais (sem duplicar) =====
            // Distinct na projeção evita multiplicação de linhas por joins
            var distinctBase = baseQuery
                .Select(l => new { l.Id, l.Amount, l.Type })
                .Distinct();

            var totalEntradas = await distinctBase
                .Where(x => x.Type == LaunchType.Income)
                .Select(x => x.Amount)
                .DefaultIfEmpty(0m)
                .SumAsync(ct);

            var totalSaidas = await distinctBase
                .Where(x => x.Type == LaunchType.Expense)
                .Select(x => x.Amount)
                .DefaultIfEmpty(0m)
                .SumAsync(ct);

            // Total de itens (Ids distintos) respeitando o filtro de tipo
            var totalItems = await filteredQuery
                .Select(l => l.Id)
                .Distinct()
                .CountAsync(ct);

            // ===== Paginação por Id com ordenação estável =====
            var pageIds = await filteredQuery
                .Select(l => new { l.Id, l.LaunchDate })
                .GroupBy(x => x.Id)
                .Select(g => new { Id = g.Key, LastDate = g.Max(x => x.LaunchDate) })
                .OrderByDescending(x => x.LastDate)
                .ThenByDescending(x => x.Id) // desempate estável
                .Skip((page - 1) * size)
                .Take(size)
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
                .ThenByDescending(i => i.Description) // opcional: estabilidade visual
                .ToListAsync(ct);

            return new PagedResultCashFlowReport
            {
                Page = page,
                PageSize = size,
                TotalItems = totalItems,
                TotalEntradas = totalEntradas,
                TotalSaidas = totalSaidas,
                Items = items
            };
        }
    }
}
