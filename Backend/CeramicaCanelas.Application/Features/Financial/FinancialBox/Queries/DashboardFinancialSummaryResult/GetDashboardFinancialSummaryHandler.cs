// GetDashboardFinancialSummaryHandler.cs - VERSÃO CORRIGIDA E OTIMIZADA

using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using Microsoft.EntityFrameworkCore; // PRECISA DESTE USING
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.Financial.FinancialBox.Queries.DashboardFinancialSummaryResult
{
    public class GetDashboardFinancialSummaryHandler : IRequestHandler<DashboardFinancialSummaryQuery, DashboardFinancialSummaryResult>
    {
        private readonly ILaunchRepository _launchRepository;

        public GetDashboardFinancialSummaryHandler(ILaunchRepository launchRepository)
        {
            _launchRepository = launchRepository;
        }

        public async Task<DashboardFinancialSummaryResult> Handle(DashboardFinancialSummaryQuery request, CancellationToken cancellationToken)
        {
            var launches = _launchRepository.QueryAllWithIncludes();

            // 🔥 CORREÇÃO 1: Usar UTC para todas as operações de data.
            var nowUtc = DateTime.UtcNow;
            var firstDayOfYear = new DateTime(nowUtc.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var last30Days = nowUtc.AddDays(-30);

            // 🔥 CORREÇÃO 2: Calcular a maioria das métricas em UMA ÚNICA CONSULTA.
            var summaryData = await launches
                .GroupBy(l => 1) // Agrupa tudo em um único resultado
                .Select(g => new
                {
                    // Totais do Ano
                    IncomeYear = g.Where(l => l.Type == LaunchType.Income && l.LaunchDate >= firstDayOfYear).Sum(l => l.Amount),
                    ExpenseYear = g.Where(l => l.Type == LaunchType.Expense && l.LaunchDate >= firstDayOfYear).Sum(l => l.Amount),

                    // Totais 30 dias
                    Income30 = g.Where(l => l.Type == LaunchType.Income && l.LaunchDate >= last30Days).Sum(l => l.Amount),
                    Expense30 = g.Where(l => l.Type == LaunchType.Expense && l.LaunchDate >= last30Days).Sum(l => l.Amount),

                    // Pendentes
                    PendingReceivables = g.Where(l => l.Type == LaunchType.Income && l.Status == PaymentStatus.Pending).Sum(l => l.Amount),
                    PendingPayments = g.Where(l => l.Type == LaunchType.Expense && l.Status == PaymentStatus.Pending).Sum(l => l.Amount),

                    // Saldo Geral
                    CurrentBalance = g.Where(l => l.Type == LaunchType.Income).Sum(l => l.Amount) - g.Where(l => l.Type == LaunchType.Expense).Sum(l => l.Amount),

                    // Clientes
                    CustomersWithLaunches = g.Where(l => l.CustomerId != null).Select(l => l.CustomerId).Distinct().Count()
                })
                .FirstOrDefaultAsync(cancellationToken); // Executa a consulta de forma assíncrona

            // Consultas que precisam ser separadas
            var lastLaunchDate = await launches
                .OrderByDescending(l => l.LaunchDate)
                .Select(l => (DateTime?)l.LaunchDate)
                .FirstOrDefaultAsync(cancellationToken);

            // 🔥 CORREÇÃO 3: Otimizar e corrigir a consulta do gráfico mensal
            var firstDayOf12MonthsAgo = new DateTime(nowUtc.Year, nowUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-11);

            var monthlyChartData = await launches
                .Where(l => l.LaunchDate >= firstDayOf12MonthsAgo)
                .GroupBy(l => new { l.LaunchDate.Year, l.LaunchDate.Month }) // Agrupa por ano/mês
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalIncome = g.Where(l => l.Type == LaunchType.Income).Sum(l => l.Amount),
                    TotalExpense = g.Where(l => l.Type == LaunchType.Expense).Sum(l => l.Amount)
                })
                .ToListAsync(cancellationToken);

            // Monta o resultado final do gráfico no lado do cliente (rápido)
            var monthlyChart = Enumerable.Range(0, 12)
                .Select(i => new DateTime(nowUtc.Year, nowUtc.Month, 1).AddMonths(-i))
                .OrderBy(m => m)
                .Select(month => {
                    var data = monthlyChartData.FirstOrDefault(d => d.Year == month.Year && d.Month == month.Month);
                    return new MonthlyCashFlowChartItem
                    {
                        Month = month.ToString("yyyy-MM"),
                        TotalIncome = data?.TotalIncome ?? 0,
                        TotalExpense = data?.TotalExpense ?? 0
                    };
                }).ToList();


            return new DashboardFinancialSummaryResult
            {
                TotalIncomeYear = summaryData?.IncomeYear ?? 0,
                TotalExpenseYear = summaryData?.ExpenseYear ?? 0,
                TotalIncome30Days = summaryData?.Income30 ?? 0,
                TotalExpense30Days = summaryData?.Expense30 ?? 0,
                PendingReceivables = summaryData?.PendingReceivables ?? 0,
                PendingPayments = summaryData?.PendingPayments ?? 0,
                CurrentBalance = summaryData?.CurrentBalance ?? 0,
                CustomersWithLaunches = summaryData?.CustomersWithLaunches ?? 0,
                LastLaunchDate = lastLaunchDate,
                MonthlyChart = monthlyChart
            };
        }
    }
}