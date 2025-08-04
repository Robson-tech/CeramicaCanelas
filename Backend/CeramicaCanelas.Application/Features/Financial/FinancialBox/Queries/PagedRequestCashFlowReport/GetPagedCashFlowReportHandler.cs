using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.PagedRequestCashFlowReport
{
    public class GetPagedCashFlowReportHandler : IRequestHandler<PagedRequestCashFlowReport, PagedResultCashFlowReport>
    {
        private readonly ILaunchRepository _launchRepository;

        public GetPagedCashFlowReportHandler(ILaunchRepository launchRepository)
        {
            _launchRepository = launchRepository;
        }

        public async Task<PagedResultCashFlowReport> Handle(PagedRequestCashFlowReport request, CancellationToken cancellationToken)
        {
            var launches = _launchRepository.QueryAllWithIncludes();

            var filtered = launches.AsQueryable();

            // Apenas lançamentos pagos devem ser considerados
            filtered = filtered.Where(l => l.Status == PaymentStatus.Paid);

            // Filtro por tipo
            if (request.type == LaunchType.Income)
                filtered = filtered.Where(l => l.Type == LaunchType.Income);
            else if (request.type == LaunchType.Expense)
                filtered = filtered.Where(l => l.Type == LaunchType.Expense);

            // Antes
            if (request.EndDate.HasValue)
                filtered = filtered.Where(l => l.LaunchDate <= request.EndDate.Value);

            // Depois
            if (request.EndDate.HasValue)
                filtered = filtered.Where(l => l.LaunchDate < request.EndDate.Value);

            // Totais apenas de lançamentos pagos
            var totalEntradas = filtered
                .Where(l => l.Type == LaunchType.Income)
                .Sum(l => l.Amount);

            var totalSaidas = filtered
                .Where(l => l.Type == LaunchType.Expense)
                .Sum(l => l.Amount);

            var totalItems = filtered.Count();

            var items = filtered
                .OrderByDescending(l => l.LaunchDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList()
                .Select(l => new CashFlowReportItem
                {
                    LaunchDate = l.LaunchDate,
                    Description = l.Description,
                    Amount = l.Amount,
                    Type = l.Type,
                    CategoryName = l.Category?.Name ?? "Sem categoria",
                    CustomerName = l.Customer?.Name ?? "Sem cliente",
                    PaymentMethod = l.PaymentMethod.ToString()
                })
                .ToList();

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
