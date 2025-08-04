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

        // GetDashboardFinancialSummaryHandler.cs - VERSÃO FINAL COM DATEONLY

        public async Task<DashboardFinancialSummaryResult> Handle(DashboardFinancialSummaryQuery request, CancellationToken cancellationToken)
        {
            var launches = _launchRepository.QueryAllWithIncludes();

            // 🔥 CORREÇÃO 1: Nossas variáveis de data agora devem ser DateOnly.
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var firstDayOfYear = new DateOnly(today.Year, 1, 1);
            var last30Days = today.AddDays(-30);

            var summaryData = await launches
                .GroupBy(l => 1)
                .Select(g => new
                {
                    // Totais do Ano (agora comparando DateOnly com DateOnly)
                    IncomeYear = g.Where(l => l.Type == LaunchType.Income && l.Status == PaymentStatus.Paid && l.LaunchDate >= firstDayOfYear).Sum(l => l.Amount),
                    ExpenseYear = g.Where(l => l.Type == LaunchType.Expense && l.Status == PaymentStatus.Paid && l.LaunchDate >= firstDayOfYear).Sum(l => l.Amount),

                    // Totais 30 dias (agora comparando DateOnly com DateOnly)
                    Income30 = g.Where(l => l.Type == LaunchType.Income && l.Status == PaymentStatus.Paid && l.LaunchDate >= last30Days).Sum(l => l.Amount),
                    Expense30 = g.Where(l => l.Type == LaunchType.Expense && l.Status == PaymentStatus.Paid && l.LaunchDate >= last30Days).Sum(l => l.Amount),

                    PendingReceivables = g.Where(l => l.Type == LaunchType.Income && l.Status == PaymentStatus.Pending).Sum(l => l.Amount),
                    PendingPayments = g.Where(l => l.Type == LaunchType.Expense && l.Status == PaymentStatus.Pending).Sum(l => l.Amount),

                    CurrentBalance = g.Where(l => l.Type == LaunchType.Income && l.Status == PaymentStatus.Paid).Sum(l => l.Amount)
                                   - g.Where(l => l.Type == LaunchType.Expense && l.Status == PaymentStatus.Paid).Sum(l => l.Amount),

                    CustomersWithLaunches = g.Where(l => l.CustomerId != null).Select(l => l.CustomerId).Distinct().Count()
                })
                .FirstOrDefaultAsync(cancellationToken);

            // 🔥 CORREÇÃO 2: A consulta da última data deve retornar DateOnly?
            var lastLaunchDate = await launches
                .OrderByDescending(l => l.LaunchDate)
                .Select(l => (DateOnly?)l.LaunchDate) // O resultado agora é DateOnly?
                .FirstOrDefaultAsync(cancellationToken);

            // 🔥 CORREÇÃO 3: A data para o gráfico também deve ser DateOnly.
            var firstDayOf12MonthsAgo = new DateOnly(today.Year, today.Month, 1).AddMonths(-11);

            var monthlyChartData = await launches
                .Where(l => l.LaunchDate >= firstDayOf12MonthsAgo) // Comparando DateOnly com DateOnly
                .GroupBy(l => new { l.LaunchDate.Year, l.LaunchDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalIncome = g.Where(l => l.Type == LaunchType.Income && l.Status == PaymentStatus.Paid).Sum(l => l.Amount),
                    TotalExpense = g.Where(l => l.Type == LaunchType.Expense && l.Status == PaymentStatus.Paid).Sum(l => l.Amount)
                })
                .ToListAsync(cancellationToken);

            // Esta parte de montar o gráfico não precisa de alteração, pois é feita em memória.
            var nowForChart = DateTime.UtcNow; // Podemos usar DateTime aqui sem problemas.
            var monthlyChart = Enumerable.Range(0, 12)
                .Select(i => new DateTime(nowForChart.Year, nowForChart.Month, 1).AddMonths(-i))
                .OrderBy(m => m)
                .Select(month =>
                {
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
                LastLaunchDate = lastLaunchDate, // ATENÇÃO AQUI - veja a próxima seção
                MonthlyChart = monthlyChart
            };
        }
    }
}