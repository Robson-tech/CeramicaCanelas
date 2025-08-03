using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Domain.Enums.Financial;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            var now = DateTime.Now;
            var firstDayOfYear = new DateTime(now.Year, 1, 1);
            var last30Days = now.AddDays(-30);

            var incomeYear = launches
                .Where(l => l.Type == LaunchType.Income && l.LaunchDate >= firstDayOfYear)
                .Sum(l => l.Amount);

            var expenseYear = launches
                .Where(l => l.Type == LaunchType.Expense && l.LaunchDate >= firstDayOfYear)
                .Sum(l => l.Amount);

            var income30 = launches
                .Where(l => l.Type == LaunchType.Income && l.LaunchDate >= last30Days)
                .Sum(l => l.Amount);

            var expense30 = launches
                .Where(l => l.Type == LaunchType.Expense && l.LaunchDate >= last30Days)
                .Sum(l => l.Amount);

            var pendingReceivables = launches
                .Where(l => l.Type == LaunchType.Income && l.Status == PaymentStatus.Pending)
                .Sum(l => l.Amount);

            var pendingPayments = launches
                .Where(l => l.Type == LaunchType.Expense && l.Status == PaymentStatus.Pending)
                .Sum(l => l.Amount);

            var currentBalance = launches
                .Where(l => l.Type == LaunchType.Income).Sum(l => l.Amount)
                -
                launches
                .Where(l => l.Type == LaunchType.Expense).Sum(l => l.Amount);

            var lastLaunchDate = launches
                .OrderByDescending(l => l.LaunchDate)
                .Select(l => (DateTime?)l.LaunchDate)
                .FirstOrDefault();

            var customersWithLaunches = launches
                .Where(l => l.CustomerId != null)
                .Select(l => l.CustomerId)
                .Distinct()
                .Count();

            // Agrupamento mensal (últimos 12 meses)
            var last12Months = Enumerable.Range(0, 12)
                .Select(i => new DateTime(now.Year, now.Month, 1).AddMonths(-i))
                .OrderBy(m => m)
                .ToList();

            var monthlyChart = last12Months.Select(month =>
            {
                var start = month;
                var end = month.AddMonths(1).AddSeconds(-1);

                return new MonthlyCashFlowChartItem
                {
                    Month = month.ToString("yyyy-MM"),
                    TotalIncome = launches.Where(l => l.Type == LaunchType.Income && l.LaunchDate >= start && l.LaunchDate <= end).Sum(l => l.Amount),
                    TotalExpense = launches.Where(l => l.Type == LaunchType.Expense && l.LaunchDate >= start && l.LaunchDate <= end).Sum(l => l.Amount)
                };
            }).ToList();


            return new DashboardFinancialSummaryResult
            {
                TotalIncomeYear = incomeYear,
                TotalExpenseYear = expenseYear,
                TotalIncome30Days = income30,
                TotalExpense30Days = expense30,
                PendingReceivables = pendingReceivables,
                PendingPayments = pendingPayments,
                CurrentBalance = currentBalance,
                LastLaunchDate = lastLaunchDate,
                CustomersWithLaunches = customersWithLaunches,
                MonthlyChart = monthlyChart
            };

        }
    }
}
