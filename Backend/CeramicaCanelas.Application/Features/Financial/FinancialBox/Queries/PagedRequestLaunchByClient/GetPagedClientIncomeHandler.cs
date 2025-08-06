using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PagedRequestLaunchByClient
{
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
                query = query.Where(l => l.LaunchDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                query = query.Where(l => l.LaunchDate <= request.EndDate.Value);

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
                    ValorPendente = _launchRepository.QueryAllWithIncludes()
                        .Where(p => p.CustomerId == key.Id && p.Status == PaymentStatus.Pending)
                        .Sum(p => p.Amount),
                    TicketMedio = group.Average(l => l.Amount)
                });

            // Executa a consulta agrupada no banco
            var groupedList = await groupedQuery.ToListAsync(cancellationToken);

            // Filtro em memória com ToLower para insensibilidade a maiúsculas/minúsculas
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchTerm = request.Search.ToLower();
                groupedList = groupedList
                    .Where(c => c.CustomerName != null && c.CustomerName.ToLower().Contains(searchTerm))
                    .ToList();
            }

            // Ordenação em memória
            groupedList = request.OrderBy?.ToLower() switch
            {
                "total" => request.Ascending
                                ? groupedList.OrderBy(c => c.TotalAmount).ToList()
                                : groupedList.OrderByDescending(c => c.TotalAmount).ToList(),

                "ticket" => request.Ascending
                                ? groupedList.OrderBy(c => c.TicketMedio).ToList()
                                : groupedList.OrderByDescending(c => c.TicketMedio).ToList(),

                "lastpurchase" => request.Ascending
                                ? groupedList.OrderBy(c => c.DataDaUltimaCompra).ToList()
                                : groupedList.OrderByDescending(c => c.DataDaUltimaCompra).ToList(),

                _ => request.Ascending
                                ? groupedList.OrderBy(c => c.CustomerName).ToList()
                                : groupedList.OrderByDescending(c => c.CustomerName).ToList(),
            };

            var totalItems = groupedList.Count;

            var pagedItems = groupedList
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

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
