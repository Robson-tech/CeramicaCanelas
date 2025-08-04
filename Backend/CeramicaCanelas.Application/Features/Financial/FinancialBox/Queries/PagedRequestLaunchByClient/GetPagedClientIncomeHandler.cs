using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PagedRequestLaunchByClient
{
    // Handler para a nova requisição
    public class GetPagedClientIncomeHandler : IRequestHandler<PagedClientIncomeRequest, PagedClientIncomeResult>
    {
        private readonly ILaunchRepository _launchRepository;

        public GetPagedClientIncomeHandler(ILaunchRepository launchRepository)
        {
            _launchRepository = launchRepository;
        }

        public async Task<PagedClientIncomeResult> Handle(PagedClientIncomeRequest request, CancellationToken cancellationToken)
        {
            var query = _launchRepository.QueryAllWithIncludes()
                .Where(l => l.Type == LaunchType.Income && l.Status == PaymentStatus.Paid && l.CustomerId != null);

            if (request.StartDate.HasValue)
                query = query.Where(l => l.LaunchDate >= request.StartDate.Value.ToUniversalTime());

            if (request.EndDate.HasValue)
                query = query.Where(l => l.LaunchDate < request.EndDate.Value.AddDays(1).ToUniversalTime());

            var groupedQuery = query.GroupBy(
                l => new
                {
                    Id = l.CustomerId.Value,
                    Name = l.Customer.Name ?? "Cliente Desconhecido"
                },
                (key, group) => new ClientIncomeSummaryResult
                {
                    CustomerId = key.Id,
                    CustomerName = key.Name,
                    TotalAmount = group.Sum(l => l.Amount),
                    QuantidadeDeCompras = group.Count(),
                    DataDaUltimaCompra = group.Max(l => l.LaunchDate),
                    ValorPendente = _launchRepository.QueryAllWithIncludes() // <-- separada para somar pendentes
                        .Where(p => p.CustomerId == key.Id && p.Status == PaymentStatus.Pending)
                        .Sum(p => p.Amount),
                    TicketMedio = group.Average(l => l.Amount)
                });

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                groupedQuery = groupedQuery.Where(c => c.CustomerName.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
            }

            groupedQuery = request.OrderBy.ToLower() switch
            {
                "total" => request.Ascending
                                ? groupedQuery.OrderBy(c => c.TotalAmount)
                                : groupedQuery.OrderByDescending(c => c.TotalAmount),
                "ticket" => request.Ascending
                                ? groupedQuery.OrderBy(c => c.TicketMedio)
                                : groupedQuery.OrderByDescending(c => c.TicketMedio),
                "lastpurchase" => request.Ascending
                                ? groupedQuery.OrderBy(c => c.DataDaUltimaCompra)
                                : groupedQuery.OrderByDescending(c => c.DataDaUltimaCompra),
                _ => request.Ascending
                                ? groupedQuery.OrderBy(c => c.CustomerName)
                                : groupedQuery.OrderByDescending(c => c.CustomerName),
            };

            var totalItems = await groupedQuery.CountAsync(cancellationToken);

            var pagedItems = await groupedQuery
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedClientIncomeResult
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                Items = pagedItems
            };
        }

    }

}