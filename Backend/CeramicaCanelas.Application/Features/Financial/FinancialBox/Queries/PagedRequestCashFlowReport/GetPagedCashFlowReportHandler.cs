using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            // Buscar dados com includes
            var launches = _launchRepository.QueryAllWithIncludes();

            // Aplicar filtros base
            var baseQuery = launches
                .Where(l => l.Status == PaymentStatus.Paid);

            // Filtros de data
            if (request.StartDate.HasValue)
                baseQuery = baseQuery.Where(l => l.LaunchDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                baseQuery = baseQuery.Where(l => l.LaunchDate <= request.EndDate.Value);

            // Filtro por descrição
            if (!string.IsNullOrWhiteSpace(request.Search))
                baseQuery = baseQuery.Where(l => l.Description.ToLower().Contains(request.Search.ToLower()));

            // Query com filtro de tipo
            var filteredQuery = baseQuery;
            if (request.type == LaunchType.Income)
                filteredQuery = baseQuery.Where(l => l.Type == LaunchType.Income);
            else if (request.type == LaunchType.Expense)
                filteredQuery = baseQuery.Where(l => l.Type == LaunchType.Expense);

            // Executar queries com Select para evitar duplicações
            var totalEntradas = await baseQuery
                .Where(l => l.Type == LaunchType.Income)
                .Select(l => l.Amount)
                .SumAsync(cancellationToken);

            var totalSaidas = await baseQuery
                .Where(l => l.Type == LaunchType.Expense)
                .Select(l => l.Amount)
                .SumAsync(cancellationToken);

            var totalItems = await filteredQuery
                .Select(l => l.Id) // Select apenas o ID para contagem
                .CountAsync(cancellationToken);

            var rawCount = await baseQuery.CountAsync(cancellationToken);
            var processedCount = totalItems;

            // Log para verificar se há diferença (use seu sistema de log ou Console)
            Console.WriteLine($"Debug - Raw count: {rawCount}, Processed count: {processedCount}");

            if (rawCount != processedCount)
            {
                Console.WriteLine("⚠️ ATENÇÃO: Ainda há duplicações detectadas!");
            }

            // Buscar items com projeção direta - isso evita duplicações
            var items = await filteredQuery
                .OrderByDescending(l => l.LaunchDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(l => new CashFlowReportItem
                {
                    LaunchDate = l.LaunchDate,
                    Description = l.Description,
                    Amount = l.Amount,
                    Type = l.Type,
                    CategoryName = l.Category != null ? l.Category.Name : "Sem categoria",
                    CustomerName = l.Customer != null ? l.Customer.Name : "Sem cliente",
                    PaymentMethod = l.PaymentMethod.ToString()
                })
                .ToListAsync(cancellationToken);

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