using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            var page = request.Page <= 0 ? 1 : request.Page;
            var size = request.PageSize <= 0 ? 10 : request.PageSize;

            // Base SEM includes
            var baseQuery = _launchRepository.QueryAll()
                .AsNoTracking()
                .Where(l => l.Status == PaymentStatus.Paid);

            // Datas
            if (request.StartDate.HasValue)
                baseQuery = baseQuery.Where(l => l.LaunchDate >= request.StartDate.Value);
            if (request.EndDate.HasValue)
                baseQuery = baseQuery.Where(l => l.LaunchDate <= request.EndDate.Value);

            // Busca
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search;
                baseQuery = baseQuery.Where(l => l.Description != null && l.Description.Contains(s));
                // Postgres case-insensitive opcional:
                // baseQuery = baseQuery.Where(l => EF.Functions.ILike(l.Description ?? "", $"%{s}%"));
            }

            // Filtro por tipo (para lista/contagem)
            var filteredQuery = baseQuery;
            if (request.type == LaunchType.Income)
                filteredQuery = filteredQuery.Where(l => l.Type == LaunchType.Income);
            else if (request.type == LaunchType.Expense)
                filteredQuery = filteredQuery.Where(l => l.Type == LaunchType.Expense);

            // ===== Totais sem duplicação (via subselect de IDs) =====
            // Pega os IDs distintos da base (respeita status/data/busca)
            var distinctIdsQuery = baseQuery.Select(l => l.Id).Distinct();

            // Soma na tabela base, filtrando por IN (um row por Id)
            var totalEntradas = await _launchRepository.QueryAll()
                .AsNoTracking()
                .Where(l => distinctIdsQuery.Contains(l.Id) && l.Type == LaunchType.Income)
                .Select(l => l.Amount)
                .DefaultIfEmpty(0m)
                .SumAsync(ct);

            var totalSaidas = await _launchRepository.QueryAll()
                .AsNoTracking()
                .Where(l => distinctIdsQuery.Contains(l.Id) && l.Type == LaunchType.Expense)
                .Select(l => l.Amount)
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
                .ThenByDescending(i => i.Description) // opcional
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
