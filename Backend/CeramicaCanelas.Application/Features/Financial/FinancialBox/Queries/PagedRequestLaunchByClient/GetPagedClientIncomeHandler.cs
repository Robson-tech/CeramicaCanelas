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
                .Where(l => l.Type == LaunchType.Income && l.CustomerId != null);

            if (request.StartDate.HasValue)
                query = query.Where(l => l.LaunchDate >= request.StartDate.Value.ToUniversalTime());

            if (request.EndDate.HasValue)
                query = query.Where(l => l.LaunchDate <= request.EndDate.Value.ToUniversalTime());

            // Agrupamento e projeção com todos os novos campos calculados
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
                    // --- NOVOS CÁLCULOS ---
                    TotalAmount = group.Sum(l => l.Amount),
                    QuantidadeDeCompras = group.Count(), // 🔥 NOVO: Conta o número de lançamentos no grupo
                    DataDaUltimaCompra = group.Max(l => l.LaunchDate), // 🔥 NOVO: Pega a data mais recente do grupo
                    ValorPendente = group.Where(l => l.Status == PaymentStatus.Pending).Sum(l => l.Amount), // 🔥 NOVO: Soma apenas os pendentes
                    TicketMedio = group.Average(l => l.Amount) // 🔥 NOVO: Calcula a média de valor por compra
                });

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                groupedQuery = groupedQuery.Where(c => c.CustomerName.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
            }

            // Adicionando as novas opções de ordenação
            groupedQuery = request.OrderBy.ToLower() switch
            {
                "total" => request.Ascending
                                ? groupedQuery.OrderBy(c => c.TotalAmount)
                                : groupedQuery.OrderByDescending(c => c.TotalAmount),
                "ticket" => request.Ascending // 🔥 NOVO
                                ? groupedQuery.OrderBy(c => c.TicketMedio)
                                : groupedQuery.OrderByDescending(c => c.TicketMedio),
                "lastpurchase" => request.Ascending // 🔥 NOVO
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